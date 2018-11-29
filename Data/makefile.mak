CAM = ..\bin\debug\edacamtool.exe
PANEL = ..\bin\debug\edacamtool.exe

all: panel3
.PHONY: all

panel3: \
	panel3_Copper$L1.gbr \
	panel3_Copper$L2.gbr \
	panel3_Legend$Top.gbr \
	panel3_Legend$Bottom.gbr \
	panel3_Soldermask$Top.gbr \
	panel3_Soldermask$Bottom.gbr \
	panel3_NonPlated$1$2$NPTH$Drill.gbr \
	panel3_Plated$1$2$PTH$Drill.gbr \
	panel3.ipc

panel3_Copper$L1.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Copper.1

panel3_Copper$L2.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Copper.2

panel3_Legend$Top.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Legend.Top

panel3_Legend$Bottom.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Legend.Bottom

panel3_Soldermask$Top.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Soldermask.Top

panel3_Soldermask$Bottom.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Soldermask.Bottom

panel3_NonPlated$1$2$NPTH$Drill.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:NonPlated.1.2.Drill

panel3_Plated$1$2$PTH$Drill.gbr: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:Plated.1.2.Drill

panel3.ipc: makefile.mak panel3.xcam panel3.xbrd
	$(CAM) panel3.xcam /t:IPC-D356
