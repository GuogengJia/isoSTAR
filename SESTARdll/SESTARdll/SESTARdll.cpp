// SESTARdll.cpp: 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include <string>
#include <iostream>
#include <fstream>
#include <vector>
#include <list>
#include <map>
#include <set>
#include <algorithm>
#include <numeric>
#include <math.h>
#include <time.h>
#include <thread>
#include <mutex>
#include <random>

using namespace std;

typedef unsigned char BYTE;

//#define ONLINE


#define PI 3.14159
#define ISOLENGTH 15
#define NEUTRON 1.0033548
#define HPLUS 1.0072765
#define HNEUTRON 1.006277
#define SECUTOFF 10.0
#define RATIOCUTOFF 6.0
#define ENVELOPEPPM 0.000010
#define ELEMENTISOLENGTH 10


#ifdef ONLINE
const int REPORTLENGTH = sizeof(double) + sizeof(short);
#else
const int STATELENGTH = sizeof(int) + sizeof(unsigned short);
const int DATALENGTH = 2 * sizeof(float) + 15 * sizeof(int) + 2 * (15 * sizeof(float));
const int REPORTLENGTH = STATELENGTH + 2 * DATALENGTH + sizeof(bool);
#endif // !ONLINE

const int EC = 6; //number of elements using in the distribution calculation
unsigned short* AcceptCharge = new unsigned short[6]{ 6,5,4,3,2,1 };
int AcceptChargeLength = 6;

int ElementNumber = 5;

//C,H,O,N,S
float  (*Element)[ELEMENTISOLENGTH] = new float[ElementNumber][ELEMENTISOLENGTH]
{
	{ 0.989,0.011,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0 },
{ 0.99985,0.00015,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0 },
{ 0.998,0.0,0.002,0.0,0.0,0.0,0.0,0.0,0.0,0.0 },
{ 0.9963,0.0037,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0 },
{ 0.9579,0.0,0.0421,0.0,0.0,0.0,0.0,0.0,0.0,0.0 },
};

double* ExactMass = NULL;
float* InitialCutoff = NULL;

enum ElementName
{
	Se=0, Dimethyl=1, Dimethyl2=2, Dimethyl3=3
};



struct EnvelopeParameters
{
	int Length;
	int Start;
	int Mass;
	bool operator<(const EnvelopeParameters& a) const
	{
		return Mass < a.Mass ? true : (Mass > a.Mass ? false : (Length < a.Length ? true : (Length > a.Length ? false : (Start < a.Start))));
	}
};




map<EnvelopeParameters, float> CutoffMap;
vector<set<EnvelopeParameters>> AcceptedParameters;


void AddElement(float distribution[ELEMENTISOLENGTH], double exactmass, float initialcutoff)
{
	float(*temp)[ELEMENTISOLENGTH] = new float[ElementNumber + 1][ELEMENTISOLENGTH];
	memcpy(temp, Element, ElementNumber * ELEMENTISOLENGTH * sizeof(float));
	memcpy(temp + ElementNumber, distribution, ELEMENTISOLENGTH * sizeof(float));

	delete[] Element;
	Element = temp;

	double* ttmp = new double[ElementNumber - 4];
	memcpy(ttmp, ExactMass, (ElementNumber-5) * sizeof(double));
	ttmp[ElementNumber-5] = exactmass;

	delete[] ExactMass;
	ExactMass = ttmp;

	float* tttmp = new float[ElementNumber - 4];
	memcpy(tttmp, InitialCutoff, (ElementNumber-5) * sizeof(float));
	tttmp[ElementNumber-5] = initialcutoff;

	delete[] InitialCutoff;
	InitialCutoff = tttmp;

	++ElementNumber;
	AcceptedParameters.push_back(set<EnvelopeParameters>());
}

void Averagine(float mass, int elementcount[EC]);

struct Rscore
{
	float R;
	int Position;

#ifndef ONLINE
	float IsotopeDistribution[ISOLENGTH];
#endif // !ONLINE

};


class Peptide
{
public:
	Peptide(float mass, int specialElementNumber = 0)
	{
		static const float averaginemass = 111.1254;
		static const float composition[EC] = { 4.9348,7.7583,1.4773,1.3577,0.0,0.0 };
		float x = mass / averaginemass;
		for (int i = 0; i < EC; i++)
		{
			ElementCount[i] = round(composition[i] * x);
		}
		ElementCount[5] = specialElementNumber;
	}

	void CalcDistribution(float IsotopeDistribution[ISOLENGTH], int element) const
	{
		for (int i = 0; i < ISOLENGTH; i++)
		{
			IsotopeDistribution[i] = 0;
		}
		IsotopeDistribution[0] = 1;

		for (int i = 0; i < EC - 1; i++)
		{
			float tmp[ISOLENGTH]{ 0 };
			memcpy(tmp, Element[i], ELEMENTISOLENGTH * sizeof(float));
			RepeatConvolve(tmp, ISOLENGTH, ElementCount[i]);
			Convolve(IsotopeDistribution, ISOLENGTH, tmp, ISOLENGTH);
		}
		float tmp[ISOLENGTH]{ 0 };
		memcpy(tmp, Element[EC - 1 + element], ELEMENTISOLENGTH * sizeof(float));
		RepeatConvolve(tmp, ISOLENGTH, ElementCount[EC - 1]);
		Convolve(IsotopeDistribution, ISOLENGTH, tmp, ISOLENGTH);
	}

private:
	int ElementCount[EC];
	void Convolve(float a[], int lena, float b[], int lenb) const
	{
		float* r = new float[lena] { 0 };
		for (int i = 0; i < lena; i++)
		{
			for (int j = 0; j < lenb; j++)
			{
				if (i + j < lena)
				{
					r[i + j] += a[i] * b[j];
				}
				else { break; }
			}
		}
		memcpy(a, r, lena * sizeof(float));
		delete[] r;
		r = NULL;
	}

	void RepeatConvolve(float a[], int length, unsigned int n) const
	{
		float* tmp = new float[length] { 0 };
		tmp[0] = 1;
		while (n)
		{
			if (n & 1)
				Convolve(tmp, length, a, length);
			Convolve(a, length, a, length);
			n >>= 1;
		}
		memcpy(a, tmp, length * sizeof(float));
		delete[] tmp;
		tmp = NULL;
	}
};


class Envelope
{
public:
	int Tag = 0b10000000000000000000000000000000;

#ifndef ONLINE
	float Error = -1;
	Envelope* Pair = NULL;
#endif

	Envelope(double mz, float intensity
#ifndef ONLINE
		, unsigned int position
#endif
		, unsigned short charge)
	{
		Add(mz, intensity
#ifndef ONLINE
			, position
#endif
			);
		Charge = charge;
	}

	void Add(double mz, float intensity
#ifndef ONLINE
		, unsigned int position
#endif
	)
	{
		if (Length >= MaxLength)
		{
			int last = MaxLength;
			MaxLength += ISOLENGTH;
			double* mtemp = new double[MaxLength] { 0 };
			float* itemp = new float[MaxLength] { 0 };
#ifndef ONLINE
			int* ptemp = new int[MaxLength] { 0 };
			memcpy(ptemp, Position, last * sizeof(int));
			for (int i = MaxLength - ISOLENGTH; i < MaxLength; i++)
			{
				ptemp[i] = -1;
			}
			delete[] Position;
			Position = ptemp;
			ptemp = NULL;
#endif
			memcpy(mtemp, Mz, last * sizeof(double));
			memcpy(itemp, Intensity, last * sizeof(float));
			

			
			delete[] Mz;
			delete[] Intensity;
			
			Mz = mtemp;
			Intensity = itemp;
			
			itemp = NULL;
			mtemp = NULL;
		}
		Mz[Length] = mz;
		Intensity[Length] = intensity;
#ifndef ONLINE
		Position[Length] = position;
#endif
		Length += 1;
	}

	BYTE* Report() const
	{
		BYTE r[REPORTLENGTH];
		

#ifdef ONLINE
		memcpy(r, Mz + GetMaxIntensityPosition(), sizeof(double));
		memcpy(r + sizeof(double), &Charge, sizeof(short));
#else
		BYTE* temp = r;
		memcpy(temp, &Tag, sizeof(Tag));
		temp += sizeof(Tag);
		memcpy(temp, &Charge, sizeof(Charge));
		temp += sizeof(Charge);

		temp = DataCopy(temp);

		if (Pair != NULL)
		{
			*temp = (BYTE)true;
			temp += sizeof(bool);
			temp = Pair->DataCopy(temp);
		}
		else *temp = (BYTE)false;
		temp = NULL;
#endif
		
		return r;
	}


	BYTE* Report(int position) const
	{
		BYTE r[REPORTLENGTH];


#ifdef ONLINE
		memcpy(r, Mz + position, sizeof(double));
		memcpy(r + sizeof(double), &Charge, sizeof(short));
#else
		BYTE* temp = r;
		memcpy(temp, &Tag, sizeof(Tag));
		temp += sizeof(Tag);
		memcpy(temp, &Charge, sizeof(Charge));
		temp += sizeof(Charge);

		temp = DataCopy(temp);

		if (Pair != NULL)
		{
			*temp = (BYTE)true;
			temp += sizeof(bool);
			temp = Pair->DataCopy(temp);
		}
		else *temp = (BYTE)false;
		temp = NULL;
#endif

		return r;
	}

#ifdef ONLINE
	BYTE* ReportIsoSeq()
	{
		BYTE r[REPORTLENGTH * 2];
		memcpy(r, Mz, sizeof(double));
		memcpy(r + sizeof(double), &Charge, sizeof(short));
		memcpy(r + REPORTLENGTH, Mz + 2, sizeof(double));
		memcpy(r + REPORTLENGTH + sizeof(double), &Charge, sizeof(short));
		return r;
	}
#endif

#ifndef ONLINE
	inline BYTE* DataCopy(BYTE* ptr) const
	{
		static const int size = ISOLENGTH * sizeof(float);

		BYTE* temp = ptr;
		float tempf;
		tempf = R[0].R;
		memcpy(temp, &tempf, sizeof(float));
		temp += sizeof(float);
		tempf = R[1].R;
		memcpy(temp, &tempf, sizeof(float));
		temp += sizeof(float);
		memcpy(temp, Position, sizeof(*Position) * ISOLENGTH);
		temp += sizeof(*Position) * ISOLENGTH;
		
		for (int i = 0; i < ISOLENGTH; ++i)
		{
			tempf = R[0].IsotopeDistribution[i];
			memcpy(temp, &tempf, sizeof(float));
			tempf = R[1].IsotopeDistribution[i];
			memcpy(temp + size, &tempf, sizeof(float));
			temp += sizeof(float);
		}

		return temp + size;
	}

	int GetPosition(int iter) const
	{
		if (iter >= Length) return -1;
		else return Position[iter];
	}
#endif

	~Envelope()
	{
		delete[] Mz;
		Mz = NULL;
		delete[] Intensity;
		Intensity = NULL;

#ifndef ONLINE
		delete[] Position;
		Position = NULL;
#endif
	}

	inline double GetMz(int iter) const
	{
		return Mz[iter];
	}

	inline float GetIntensity(int iter) const
	{
		return Intensity[iter];
	}

	inline double GetLastMz() const
	{
		return Mz[Length - 1];
	}

	inline int GetLength() const
	{
		return Length;
	}

	inline int GetCharge() const
	{
		return Charge;
	}

	inline int GetMaxIntensityPosition() const
	{
		return max_element(Intensity, Intensity + Length) - Intensity;
	}

	inline float GetMaxIntensity() const
	{
		return *max_element(Intensity, Intensity + Length);
	}

	inline double GetMaxMz() const
	{
		return Mz[GetMaxIntensityPosition()];
	}

	inline Rscore GetR(int iter) const
	{
		if (iter > 1) return Rscore{ -1, -1 };
		return R[iter];
	}

	Envelope(const Envelope& e)
	{
		MaxLength = e.MaxLength;
		Length = e.Length;
		Charge = e.Charge;

		Mz = new double[MaxLength];
		Intensity = new float[MaxLength];
		memcpy(Mz, e.Mz, MaxLength * sizeof(double));
		memcpy(Intensity, e.Intensity, MaxLength * sizeof(float));

#ifndef ONLINE
		Position = new int[MaxLength];
		memcpy(Position, e.Position, MaxLength * sizeof(int));
#endif
	}

	Envelope(const Envelope& e, int start, int length)
	{
		MaxLength = e.MaxLength;
		Length = length;
		Charge = e.Charge;

		Mz = new double[MaxLength];
		Intensity = new float[MaxLength];
		memcpy(Mz, e.Mz + start, length * sizeof(double));
		memcpy(Intensity, e.Intensity + start, length * sizeof(float));

#ifndef ONLINE
		Position = new int[MaxLength];
		memcpy(Position, e.Position + start, length * sizeof(int));
#endif

		for (int i = length; i < MaxLength; i++)
		{
			Mz[i] = 0;
			Intensity[i] = 0;
#ifndef ONLINE
			Position[i] = -1;
#endif
		}
	}


	Rscore* JudgePattern(int element, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF)
	{
		Rscore* r = NULL;
		int p = GetMaxIntensityPosition();

		double mass = Mz[GetMaxIntensityPosition()] * Charge;
		Peptide se(mass - ExactMass[element], 1), normal(mass);
		R[0] = Rate(se, Length, ISOLENGTH - Length, element);
		R[1] = Rate(normal, Length, ISOLENGTH - Length, element);

		EnvelopeParameters Parameters{ 0,0,0 };
		Parameters.Length = Length;
		Parameters.Mass = ((int)round(Mz[GetMaxIntensityPosition()] * Charge / 100)) * 100;
		Parameters.Start = R[0].Position;
		if (R[0].R < seCutoff && R[1].R / R[0].R > normalCutoff && AcceptedParameters[element].find(Parameters) != AcceptedParameters[element].end())
		{
			r = &R[0];
			Tag &= 0b01111111111111111111111111111111;
		}

		return r;
	}

	Rscore* JudgeEnvelope(float intensity[], int length)
	{
		R[0] = Rate(intensity, min(Length, length), 0);

		return &R[0];
	}

	Rscore* JudgeMeta()
	{
		double mass = Mz[GetMaxIntensityPosition()] * Charge;
		Peptide meta(mass);
		R[0] = Rate(meta, Length, 0, Se);

		return &R[0];
	}

	bool JudgeMS2Pair(int massShift)
	{
		int p1 = -1;
		int p2 = -1;

		float* intensitycopy = new float[Length + 2]();
		
		memcpy(intensitycopy + 1, Intensity, Length * sizeof(float));

		for (int i = 1; i <= Length; ++i)
		{
			if (intensitycopy[i] > intensitycopy[i - 1] && intensitycopy[i] > intensitycopy[i + 1])
			{
				if (p1 == -1)
				{
					p1 = i;
				}
				else
				{
					p2 = i;
					break;
				}
			}
		}
		if (p2 - p1 <= massShift + 1 && p2 - p1 >= massShift - 1 && min(intensitycopy[p1], intensitycopy[p2]) / max(intensitycopy[p1], intensitycopy[p2]) > 0.7)
		{
			delete[] intensitycopy;
			return true;
		
		}
		delete[] intensitycopy;
		return false;
	}

	Envelope* Filter(float ratio)
	{
		double maxIntens = GetMaxIntensity();
		int start = 0;
		for (int i = 0; i < Length; ++i)
		{
			if (Intensity[i] > maxIntens * ratio)
			{
				start = i;
				break;
			}
		}


		return new Envelope(*this, start, Length - start);
	}


private:
	unsigned short MaxLength = 0;
	double* Mz = NULL;
	float* Intensity = NULL;
	short Charge;
	unsigned short Length = 0;
	Rscore R[2];

#ifndef ONLINE
	int* Position = NULL;
#endif // !ONLINE

	Rscore Rate(Peptide& peptide, int length, int maxShift, int element)
	{
		Rscore r{ -1 };
		float isodis[ISOLENGTH];
		peptide.CalcDistribution(isodis, element);

		float z = 0;
		for (int j = 0; j < length; j++)
		{
			z += Intensity[j] * Intensity[j];
		}

		for (int i = 0; i <= maxShift; i++)
		{
			float x = 0, y = 0;
			//y=0;z=0;
			for (int j = 0; j < length; j++)
			{
				x += Intensity[j] * isodis[i + j];
				y += isodis[i + j] * isodis[i + j];
			}
			float temp = acos(x / (sqrt(y) * sqrt(z))) * 180.0 / PI;
			if (r.R == -1 || temp < r.R)
			{
				r.R = temp;
				r.Position = i;
			}
		}

#ifndef ONLINE
		for (int i = 0; i < Length; i++)
		{
			r.IsotopeDistribution[i] = isodis[i + r.Position];
		}
#endif
		return r;
	}

	Rscore Rate(float isodis[], int length, int maxShift)
	{
		Rscore r{ -1 };

		float z = 0;
		for (int j = 0; j < length; j++)
		{
			z += Intensity[j] * Intensity[j];
		}

		for (int i = 0; i <= maxShift; i++)
		{
			float x = 0, y = 0;
			for (int j = 0; j < length; j++)
			{
				x += Intensity[j] * isodis[i + j];
				y += isodis[i + j] * isodis[i + j];
			}
			float temp = acos(x / (sqrt(y) * sqrt(z))) * 180.0 / PI;
			if (r.R == -1 || temp < r.R)
			{
				r.R = temp;
				r.Position = i;
			}
		}

#ifndef ONLINE
		for (int i = 0; i < Length; i++)
		{
			r.IsotopeDistribution[i] = isodis[i + r.Position];
		}
#endif
		return r;
	}

};


inline bool ComparePPM(double left, double right, double ppm)
{
	return abs(left - right) / (left + right) < ppm / 2;
}


void Averagine(float mass, int elementcount[EC])
{
	static const float averaginemass = 111.1254;
	static const float composition[EC] = { 4.9348,7.7583,1.4773,1.3577,0.0,0.0 };
	float x = mass / averaginemass;
	for (int i = 0; i < EC; ++i)
	{
		elementcount[i] = round(composition[i] * x);
	}
}

#ifndef ONLINE
void Deconvolve(float result[ISOLENGTH], float normalDistribution[ISOLENGTH], float distribution[ISOLENGTH])
{
	for (int i = 0; i < ISOLENGTH; ++i)
	{
		result[i] = distribution[i] / normalDistribution[0];
		for (int j = i; j < ISOLENGTH; ++j)
		{
			distribution[j] -= result[i] * normalDistribution[j - i];
		}
	}
}
#endif

void FindEnvelope(vector<Envelope*>& envelopes, double mz[], double intensity[], int length, int lenMin = 6, int lenMax = 14)
{
	vector<Envelope*> temp;
	temp.reserve(length * 1.5f);
	Envelope* e = NULL;
	unsigned short charge;
	for (int k = 0; k < AcceptChargeLength; ++k)
	{
		charge = AcceptCharge[k];
		temp.clear();
		
		for (int i = 0; i < length; i++)
		{
			if (mz[i] == -1) continue;
			bool ff = false;
			double mass = mz[i] * charge;
			for (int j = temp.size() - 1; j >= 0; j--)
			{
				if (temp[j] == NULL) continue;
				double lastmass = (*temp[j]).GetLastMz() * (*temp[j]).GetCharge();
				double last2mass = (*temp[j]).GetMz((*temp[j]).GetLength() - 2) * (*temp[j]).GetCharge();
				if (abs(lastmass + NEUTRON - mass) < ENVELOPEPPM * (mass + lastmass) / 2.0)
				{
#ifdef ONLINE
					(*temp[j]).Add(mz[i], intensity[i]);
#else
					(*temp[j]).Add(mz[i], intensity[i], i);
#endif
					temp.push_back(temp[j]);
					temp[j] = NULL;
					ff = true;
				}
				else if (abs(last2mass + NEUTRON - mass) < ENVELOPEPPM * (mass + last2mass) / 2.0)
				{
					e = new Envelope(*temp[j], 0, (*temp[j]).GetLength() - 1);
#ifdef ONLINE
					(*e).Add(mz[i], intensity[i]);
#else
					(*e).Add(mz[i], intensity[i], i);
#endif
					temp.push_back(e);
					ff = true;
				}
				else if (lastmass < (mass - NEUTRON) * (1.0 - ENVELOPEPPM))
				{
					break;
				}
			}
			if (!ff)
			{
#ifdef ONLINE
				e = new Envelope(mz[i], intensity[i], charge);
#else
				e = new Envelope(mz[i], intensity[i], i, charge);
#endif
				temp.push_back(e);
			}
		}
		e = NULL;
		
		int counttemp = 0;
		for (int j = temp.size() - 1; j >= 0; j--)
		{
			if (temp[j] == NULL) continue;
			counttemp += 1;

			int len = (*temp[j]).GetLength();
			bool addflag = false;
			if (len >= lenMin && len <= lenMax)
			{
				addflag = true;
				for (vector<Envelope*>::iterator k = envelopes.begin(); k != envelopes.end(); k++)
				{
					bool containflag = true;
					for (int jj = 0; jj < len; jj++)
					{
						bool uniqueflag = true;
						for (int kk = 0; kk < (**k).GetLength(); kk++)
						{
							if ((*temp[j]).GetMz(jj) == (**k).GetMz(kk))
							{
								uniqueflag = false;
								break;
							}
						}
						if (uniqueflag)
						{
							containflag = false;
							break;
						}
					}
					if (containflag)
					{
						addflag = false;
						break;
					}
				}
			}
			if (!addflag)
			{
				delete temp[j];
				temp[j] = NULL;
			}
			else
			{
				envelopes.push_back(temp[j]);
			}

		}
	}
}



void FindPatternEnvelope(int element, vector<Envelope*>& seEnvelopes, const vector<Envelope*>& envelopes, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF, int lenMin = 6, int lenMax = 14)
{
	for (auto i = envelopes.begin(); i != envelopes.end(); i++)
	{
		if ((**i).GetLength() >= lenMin && (**i).GetLength() <= lenMax && (**i).GetMaxMz()*(**i).GetCharge()>100+ExactMass[element] && (**i).JudgePattern(element, normalCutoff, seCutoff) != NULL)
		{
			seEnvelopes.push_back(*i);
		}
	}

}



void EnvelopeFilter(vector<Envelope*>& envelopeFiltered, const vector<Envelope*>& envelopes, float ratio)
{
	for (auto i = envelopes.begin(); i != envelopes.end(); i++)
	{
		envelopeFiltered.push_back((**i).Filter(ratio));
	}
}


extern "C" __declspec(dllexport) BYTE* Report(int* count, double mz[], double intensity[], int length, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF, int lenmin = 6, int lenmax = 14)
{
	vector<Envelope*> envelopes;
	FindEnvelope(envelopes, mz, intensity, length, lenmin, lenmax);

	vector<Envelope*> seEnvelopes;
	FindPatternEnvelope(Se, seEnvelopes, envelopes, normalCutoff, seCutoff, lenmin, lenmax);

	*count = seEnvelopes.size();
	BYTE* r = NULL;

	if (*count != 0)
	{
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;

		for (vector<Envelope*>::iterator i = seEnvelopes.begin(); i != seEnvelopes.end(); i++)
		{
			memcpy(temp, (**i).Report(), REPORTLENGTH);
			temp += REPORTLENGTH + sizeof(bool);
			if ((**i).GetR(0).R < 10)
			{
				*(temp - sizeof(bool)) = 1;
			}	
			else
			{
				*(temp - sizeof(bool)) = 0;
			}
				
		}

		temp = NULL;
	}

	for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
	{
		delete* i;
		*i = NULL;
	}

	return r;
}


extern "C" __declspec(dllexport) BYTE * ReportAnyPattern(int* count, double mz[], double intensity[], int length, int element=Se, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF, int lenmin = 6, int lenmax = 14)
{
	vector<Envelope*> envelopes;
	FindEnvelope(envelopes, mz, intensity, length, lenmin, lenmax);

	vector<Envelope*> seEnvelopes;
	FindPatternEnvelope(element, seEnvelopes, envelopes, normalCutoff, seCutoff, lenmin, lenmax);

	*count = seEnvelopes.size();
	BYTE* r = NULL;

	if (*count != 0)
	{
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;

		for (vector<Envelope*>::iterator i = seEnvelopes.begin(); i != seEnvelopes.end(); i++)
		{
			memcpy(temp, (**i).Report(), REPORTLENGTH);
			temp += REPORTLENGTH + sizeof(bool);
			if ((**i).GetR(0).R < 10)
			{
				*(temp - sizeof(bool)) = 1;
			}
			else
			{
				*(temp - sizeof(bool)) = 0;
			}

		}

		temp = NULL;
	}

	for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
	{
		delete* i;
		*i = NULL;
	}

	return r;
}


extern "C" __declspec(dllexport) BYTE * ReportMulti(int* count, double mz[], double intensity[], int length, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF, int lenmin = 6, int lenmax = 14)
{
	vector<Envelope*> envelopes;
	FindEnvelope(envelopes, mz, intensity, length, lenmin, lenmax);
	cout << "size: " << envelopes.size() << endl;
	int patterns[]{ Dimethyl, Dimethyl2, Dimethyl3 };

	vector<Envelope*> seEnvelopes;
	for (int i = 0; i < 3; ++i)
		FindPatternEnvelope(patterns[i], seEnvelopes, envelopes, normalCutoff, seCutoff, lenmin, lenmax);

	*count = seEnvelopes.size();
	BYTE* r = NULL;

	if (*count != 0)
	{
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;

		for (vector<Envelope*>::iterator i = seEnvelopes.begin(); i != seEnvelopes.end(); i++)
		{
			memcpy(temp, (**i).Report(), REPORTLENGTH);
			temp += REPORTLENGTH + sizeof(bool);
			if ((**i).GetR(0).R < 10)
			{
				*(temp - sizeof(bool)) = 1;
			}
			else
			{
				*(temp - sizeof(bool)) = 0;
			}

		}

		temp = NULL;
	}

	for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
	{
		delete* i;
		*i = NULL;
	}

	return r;
}

extern "C" __declspec(dllexport) BYTE* ReportEnvelope(int* count, double mz[], double intensity[], int length, float normalCutoff = RATIOCUTOFF, float seCutoff = SECUTOFF, int lenmin = 3, int lenmax = 14)
{
	vector<Envelope*> envelopes;
	FindEnvelope(envelopes, mz, intensity, length, lenmin, lenmax);

	sort(envelopes.begin(), envelopes.end(), [](Envelope* a, Envelope* b) -> bool { return a->GetMaxIntensity() > b->GetMaxIntensity(); });

	*count = envelopes.size();

	BYTE* r = NULL;

	if (*count != 0)
	{
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;

		for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
		{
			memcpy(temp, (**i).Report(), REPORTLENGTH);
			temp += REPORTLENGTH + sizeof(bool);

			*(temp - sizeof(bool)) = 0;
		}

		temp = NULL;
	}

	for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
	{
		delete* i;
		*i = NULL;
	}

	return r;
}

extern "C" __declspec(dllexport) BYTE * ReportMS2Diag(int* count, double mz[], double intensity[], int length, float lowerRatio = RATIOCUTOFF, float higherRatio = SECUTOFF, int lenmin = 3, int lenmax = 14)
{
	float m254 = 0;
	float m256 = 0;
	float m313 = 0;

	for (int i =0;i<length;++i)
	{
		if (ComparePPM(mz[i], 254.1499, 0.00002))
			m254 = intensity[i];
		if (ComparePPM(mz[i], 256.15663, 0.00002))
			m256 = intensity[i];
		if (ComparePPM(mz[i], 313.17809, 0.00002))
			m313 = intensity[i];
	}
	BYTE* r = NULL;

	*count = 0;
	if (m254 * m256 * m313 != 0 && min(m254, m256) / max(m254, m256) > 0.5)
	{
		*count = 1;
#ifdef ONLINE
		Envelope* e = new Envelope(313.17, 1000, 1);
#else
		Envelope* e = new Envelope(313.17, 1000, 1,1);
#endif
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;
		memcpy(temp, (*e).Report(), REPORTLENGTH);
		temp += REPORTLENGTH + sizeof(bool);
		*(temp - sizeof(bool)) = 0;
		delete e;
	}

	return r;
}

extern "C" __declspec(dllexport) BYTE * ReportMS2RBP(int* count, double mz[], double intensity[], int length, float lowerRatio = RATIOCUTOFF, float higherRatio = SECUTOFF, int lenmin = 3, int lenmax = 14)
{
	vector<Envelope*> envelopes;
	FindEnvelope(envelopes, mz, intensity, length, lenmin, lenmax);

	vector<Envelope*> diagEnvelopes;

	for (Envelope* e : envelopes)
	{
		if (e->GetLength() < 4)
			continue;
		float ratio = e->GetIntensity(2) / e->GetIntensity(0);
		float ratio2 = e->GetIntensity(3) / e->GetIntensity(1);
		if (e->GetCharge() < 3 && ratio > 1.5 && ratio < 3.0 && ratio2 > 1.0 && ratio2 < 3.0)
		{
			diagEnvelopes.push_back(e);
		}
	}

	*count = diagEnvelopes.size();


	BYTE* r = NULL;

	if (*count != 0)
	{
		r = new BYTE[*count * (REPORTLENGTH + sizeof(bool))];
		BYTE* temp = r;

		for (vector<Envelope*>::iterator i = diagEnvelopes.begin(); i != diagEnvelopes.end(); i++)
		{
			memcpy(temp, (**i).Report(), REPORTLENGTH);
			temp += REPORTLENGTH + sizeof(bool);

			*(temp - sizeof(bool)) = 0;
		}

		temp = NULL;
	}

	for (vector<Envelope*>::iterator i = envelopes.begin(); i != envelopes.end(); i++)
	{
		delete* i;
		*i = NULL;
	}

	return r;
}


void Difference(int length, int mass, int element, float factor = 0.2)
{
	Peptide pattern((double)mass - ExactMass[element], 1);
	Peptide normal(mass);

	float isodisPattern[ISOLENGTH];
	pattern.CalcDistribution(isodisPattern, element);

	float isodisNormal[ISOLENGTH];
	normal.CalcDistribution(isodisNormal, element);


	for (; length <= ISOLENGTH; ++length)
	{
		Rscore r = Rscore{ -1,-1 };
		for (int i = 0; i < ISOLENGTH - length; ++i)
		{
			double outmax = 0, inmin = 1;
			for (int j = 0; j < ISOLENGTH; ++j)
			{
				if (j < i || j >= i + length) outmax = max(isodisPattern[j], outmax);
				else inmin = min(isodisPattern[j], inmin);
			}
			if (inmin * (1.0 + factor) < outmax * (1.0 - factor)) continue;


			float z = 0;
			for (int j = 0; j < length; ++j)
			{
				z += isodisPattern[i + j] * isodisPattern[i + j];
			}

			int diff = ISOLENGTH - length;
			for (int j = 0; j <= diff; ++j)
			{
				float x = 0, y = 0;
				for (int k = 0; k < length; ++k)
				{
					x += isodisPattern[i + k] * isodisNormal[j + k];
					y += isodisNormal[j + k] * isodisNormal[j + k];
				}

				float temp = acos(x / (sqrt(y) * sqrt(z))) * 180.0 / PI;
				temp = isnan(temp) ? 0 : temp;
				if (r.R == -1 || temp < r.R)
				{
					r.R = temp;
					r.Position = j;
				}
			}

			CutoffMap[EnvelopeParameters{ length, i, mass }] = r.R;

			cout << mass << " " << length << " " << i << " " << r.R << endl;
		}
	}

}


void CalcCutoff(int massMin, int massMax, int lengthMin)
{
	massMin = massMin / 100 * 100;
	massMax = ceil(massMax / 100) * 100;

	AcceptedParameters.clear();

	for (int iii = 0; iii < ElementNumber - 5; ++iii)
	{
		CutoffMap.clear();

		set<EnvelopeParameters> temp;
		for (int mass = massMin; mass <= massMax; mass += 100)
		{
			Difference(lengthMin, mass, iii);
		}

		for (const auto &p : CutoffMap)
		{
			if (p.second > InitialCutoff[iii])
			{
				temp.insert(p.first);
				cout << "mass: " << p.first.Mass << " pos:" << p.first.Start << " length:" << p.first.Length << endl;
			}
		}
		AcceptedParameters.push_back(temp);
		
		
	}
	
}

extern "C" __declspec(dllexport) int Initialize(int massrangelow = 300, int massrangehigh = 5000)
{
	ifstream infile("params.dat", ios::in | ios::binary);
	if (!infile)
	{
		CalcCutoff(massrangelow, massrangehigh, 5);
		return 1;
	}

	infile.read((char*)&AcceptChargeLength, sizeof(int));
	infile.read((char*)AcceptCharge, sizeof(unsigned short) * AcceptChargeLength);

	for (int i = 0; i < AcceptChargeLength; i++)
	{
		cout << AcceptCharge[i] << ",";
	}
	cout << endl;

	AcceptedParameters.clear();
	EnvelopeParameters p;
	float elementiso[ELEMENTISOLENGTH];
	float cutoff;
	double mass;
	while (infile.read((char*)& p, sizeof(p)))
	{
		if (p.Length == -1)
		{
			infile.read((char*)elementiso, sizeof(float) * ELEMENTISOLENGTH);
			infile.read((char*)&mass, sizeof(double));
			infile.read((char*)&cutoff, sizeof(float));
			AddElement(elementiso, mass, cutoff);
		}
		else
		{
			(AcceptedParameters.end() - 1)->insert(p);
		}
	}

	for (auto pp : AcceptedParameters)
	{
		cout << "special:::" << endl;
		for (const auto &p : pp)
			cout << "mass: " << p.Mass << " pos:" << p.Start << " length:" << p.Length << endl;
	}
		
	infile.close();

	return 1;
}


extern "C" __declspec(dllexport) int AddElementExtern(float distribution[ELEMENTISOLENGTH], double exactmass, float initialcutoff)
{
	AddElement(distribution, exactmass, initialcutoff);
	return 1;
}

extern "C" __declspec(dllexport) int ReInitialize(int massrangelow = 300, int massrangehigh = 5000, int minLength = 5)
{
	CalcCutoff(massrangelow, massrangehigh, minLength);
	return 1;
}

extern "C" __declspec(dllexport) int SaveParams()
{
	ofstream outfile("params.dat", ios::out | ios::binary);
	outfile.write((char*)&AcceptChargeLength, sizeof(int));
	outfile.write((char*)AcceptCharge, sizeof(unsigned short) * AcceptChargeLength);

	EnvelopeParameters startp{ -1,-1,-1 };

	for (int iii = 0; iii < ElementNumber - 5; ++iii)
	{
		outfile.write((char*)&startp, sizeof(startp));
		outfile.write((char*)(Element + 5 + iii), sizeof(float) * ELEMENTISOLENGTH);
		outfile.write((char*)(ExactMass + iii), sizeof(double));
		outfile.write((char*)(InitialCutoff + iii), sizeof(float));
		for (const auto& p : AcceptedParameters[iii])
		{
			outfile.write((char*)&p, sizeof(p));
		}
	}
	outfile.close();
	return 1;
}

extern "C" __declspec(dllexport) int ChangeAcceptCharge(unsigned short* acceptmass, int length)
{
	memcpy(AcceptCharge, acceptmass, length * sizeof(unsigned short));
	AcceptChargeLength = length;
	sort(AcceptCharge, AcceptCharge + AcceptChargeLength, greater<unsigned short>());
	for (int i=0;i<length;i++)
	{
		cout << AcceptCharge[i]<<",";
	}
	cout << endl;
	return 1;
}