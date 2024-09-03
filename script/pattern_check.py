import os

print("please enter folder's path")
path=input()

ms1check_result={}
ms2check_result={}

for f in os.listdir(path):
    if f.endswith("ms1"):
        name1=".".join(f.split(".")[:-1])+".pepXML"
        name2=".".join(f.split(".")[:-1])+"_charged.pepXML"
        result=[]
        
        file=open(path+"\\"+f+"_result.txt","r")
        file.readline()
        for line in file.readlines():
            words=line.split("\t")
            result.append([float(words[0])*60.0,float(words[3]),int(words[1])])
        file.close()
        ms1check_result[".".join(f.split(".")[:-1])+".pepXML"]=result
        ms1check_result[".".join(f.split(".")[:-1])+"_charged.pepXML"]=result

    if f.endswith("ms2"):
        name1=".".join(f.split(".")[:-1])+".pepXML"
        name2=".".join(f.split(".")[:-1])+"_charged.pepXML"
        result=[]

        sw=open(path+"\\"+f+"_result.txt","r")
        sw.readline()
        ms2check_result[".".join(f.split(".")[:-1])+".pepXML"]=set()
        ms2check_result[".".join(f.split(".")[:-1])+"_charged.pepXML"]=set()
        for line in sw.readlines():
            words=line.split("\t")        
            ms2check_result[".".join(f.split(".")[:-1])+".pepXML"].add(int(words[9]))
            ms2check_result[".".join(f.split(".")[:-1])+"_charged.pepXML"].add(int(words[9]))
        sw.close()
        print(f+" end")

for root,dirs,files in os.walk(path):
        xmlname=""
        for f in files:
            if f.endswith("_c.pepXML"):
                continue
            if f.endswith(".pepXML"):
                xmlname=f
        if xmlname=="":
            continue
        if "psm_filtered.tsv" in files:
            print("checking "+root+"\\"+xmlname)
            psmfile=open(root+"\\psm_filtered.tsv","r")
            fisrtline=psmfile.readline().strip()
            newpsmfile=open(root+"\\psm_filtered_checked.tsv","w")
            newpsmfile.write(fisrtline+"\tMS2 check\tMS1 check\n")

            psms=[]
            for line in psmfile.readlines():
                words=line.split("\t")
                psms.append([float(words[6]),words[1],line])

            psms.sort()
            newpsms=[]

            for p in psms:
                for np in newpsms:
                    flag=False
                    for d in range(min(4,np[3]+2)):
                        if abs(p[0]-d*1.0033548-sum(np[0])/len(np[0]))<0.01:
                            np[0].append(p[0]-d*1.0033548)
                            if d in np[4].keys():
                                np[4][d].append(p[0])
                            else:
                                np[4][d]=[p[0]]
                            flag=True
                            np[3]=max(np[3],d)
                            np[2].append(p[2])
                            break
                    if flag:
                        break
                            
                else:
                    newpsms.append([[p[0]],p[1],[p[2]],0,{0:[p[0]]}])
            
            newlines=[]
            for np in newpsms:
                for line in np[2]:
                    words=line.strip("\n").split("\t")
                    newlines.append("\t".join(words)+"\n")


            if xmlname in ms2check_result.keys():
                for i in range(len(newlines)):
                    words=newlines[i].split("\t")
                    line=newlines[i]
                    if int(words[0].split(".")[1]) in ms2check_result[xmlname]:
                        newlines[i]=line.strip("\n")+"\tTrue\n"
                    else:
                        newlines[i]=line.strip("\n")+"\tFalse\n"
            else:
                for i in range(len(newlines)):
                    line=newlines[i]
                    newlines[i]=line.strip("\n")+"\tN/A\n"

            
            if xmlname in ms1check_result.keys():
                for i in range(len(newlines)):
                    words=newlines[i].split("\t")
                    line=newlines[i]
                    ms1flag=False
                    for pattern in ms1check_result[xmlname]:
                        if pattern[2]==int(words[7]) and abs(float(words[8])-pattern[0])<30:
                            for n in (-1,0,1):
                                if abs(abs(float(words[9])-pattern[1])-n*1.0033548/pattern[2])/(float(words[11])+pattern[1])*2<0.000025:
                                    ms1flag=True
                                    break
                            if ms1flag:
                                break
                    if ms1flag:
                        newlines[i]=line.strip("\n")+"\tTrue\n"
                    else:
                        newlines[i]=line.strip("\n")+"\tFalse\n"
            else:
                for i in range(len(newlines)):
                    line=newlines[i]
                    newlines[i]=line.strip("\n")+"\tN/A\n"
            
            for line in newlines:
                newpsmfile.write(line)
            newpsmfile.close()
                

        if "psm.tsv" in files:
            print("checking "+root+"\\"+xmlname)
            psmfile=open(root+"\\psm.tsv","r")
            newpsmfile=open(root+"\\psm_checked.tsv","w")
            fisrtline=psmfile.readline().strip()
            length=len(fisrtline.split("\t"))
            newpsmfile.write(fisrtline+"\tMS2 check\tMS1 check\n")

            psms=[]
            for line in psmfile.readlines():
                words=line.split("\t")
                psms.append([float(words[15]),words[2],line.strip("\n")+(length-len(words))*"\t"])

            psms.sort()
            newpsms=[]

            for p in psms:
                for np in newpsms:
                    flag=False
                    for d in range(min(4,np[3]+2)):
                        if abs(p[0]-d*1.0033548-sum(np[0])/len(np[0]))<0.01:
                            np[0].append(p[0]-d*1.0033548)
                            if d in np[4].keys():
                                np[4][d].append(p[0])
                            else:
                                np[4][d]=[p[0]]
                            flag=True
                            np[3]=max(np[3],d)
                            np[2].append(p[2])
                            break
                    if flag:
                        break
                            
                else:
                    newpsms.append([[p[0]],p[1],[p[2]],0,{0:[p[0]]}])
            
            newlines=[]
            for np in newpsms:
                for line in np[2]:
                    words=line.strip("\n").split("\t")
                    newlines.append("\t".join(words)+"\n")
    
            if xmlname in ms2check_result.keys():
                for i in range(len(newlines)):
                    line=newlines[i]
                    words=newlines[i].split("\t")

                    if int(words[0].split(".")[1]) in ms2check_result[xmlname]:
                        newlines[i]=line.strip("\n")+"\tTrue\n"
                    else:
                        newlines[i]=line.strip("\n")+"\tFalse\n"
            else:
                for i in range(len(newlines)):
                    line=newlines[i]
                    newlines[i]=line.strip("\n")+"\tN/A\n"

            if xmlname in ms1check_result.keys():
                for i in range(len(newlines)):
                    line=newlines[i]
                    words=newlines[i].split("\t")
                    ms1flag=False
                    for pattern in ms1check_result[xmlname]:
                        if pattern[2]==int(words[7]) and abs(float(words[8])-pattern[0])<30:
                            for n in (-1,0,1):
                                if abs(abs(float(words[11])-pattern[1])-n*1.0033548/pattern[2])/(float(words[11])+pattern[1])*2<0.000025:
                                    ms1flag=True
                                    break
                            if ms1flag:
                                break
                    if ms1flag:
                        newlines[i]=line.strip("\n")+"\tTrue\n"
                    else:
                        newlines[i]=line.strip("\n")+"\tFalse\n"
            else:
                for i in range(len(newlines)):
                    line=newlines[i]
                    newlines[i]=line.strip("\n")+"\tN/A\n"
            
            for line in newlines:
                newpsmfile.write(line)
            newpsmfile.close()
        
print("end")
