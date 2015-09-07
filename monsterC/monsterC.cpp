// General 'include' statements
#include <conio.h>
#include <malloc.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "stdafx.h"
// GPIB 'include' statements
#include <windows.h>
#include "decl-32.h"

#define GPIB0		0	// Board handle
#define BDINDEX	0
#define PRIMARY_ADDRESS 12
#define SECONDARY_ADDRESS 0
#define TIMEOUT	T10s
#define EOTMODE	1
#define EOSMODE	0
int i;
int rslt;
int	Dev;		// Device handle
int nr_pt, pt_off;
char c;
float yoff, ymult, xincr;
FILE* outfile;
char wfm[1040];
char timenow[12], date[12], xunit[12], yunit[12];
char ReadBuffer[80];
char ErrorMnemonic[21][5] = { "EDVR", "ECIC", "ENOL", "EADR", "EARG",
"ESAC", "EABO", "ENEB", "EDMA", "",
"EOIP", "ECAP", "EFSO", "", "EBUS",
"ESTB", "ESRQ", "", "", "", "ETAB" };

// If a GPIB call fails, this procedure prints an error message, offlines 
// the board, and exits.
void GPIBCleanup(int ud, char* ErrorMsg);

void GPIBCleanup(int ud, char* ErrorMsg)
{
	printf("Error : %s\nibsta = 0x%x iberr = %d (%s)\n",
		ErrorMsg, ibsta, iberr, ErrorMnemonic[iberr]);
	printf("Cleanup: Taking board offline\n");
	ibonl(ud, 0);
}

void d_GPIBCleanup(int ud, char* ErrorMsg);

void d_GPIBCleanup(int ud, char* ErrorMsg)
{
	printf("Error : %s\nibsta = 0x%x iberr = %d (%s)\n",
		ErrorMsg, ibsta, iberr, ErrorMnemonic[iberr]);
	if (ud != -1)
	{
		printf("Cleanup: Taking device offline\n");
		ibonl(ud, 0);
	}
}

int _cdecl main(void) {
	// Ensure that the board being used is Controller-In-Charge

	SendIFC(GPIB0);
	if (ibsta & ERR)
	{
		GPIBCleanup(GPIB0, "Unable to open board");
		return 1;
	}

	// Assign the scope an identifier

	int Dev = ibdev(BDINDEX, PRIMARY_ADDRESS, SECONDARY_ADDRESS,
		TIMEOUT, EOTMODE, EOSMODE);
	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to open device");
		return 1;
	}

	ibclr(Dev);
	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to clear device");
		return 1;
	}

	// Preliminary querying

	printf("Querying scope with [*IDN?] ...\n");

	ibwrt(Dev, "*IDN?", 5L);
	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to write to device");
		return 1;
	}

	ibrd(Dev, ReadBuffer, 80);
	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to read data from device");
		return 1;
	}

	ReadBuffer[ibcntl] = '\0';
	printf("%s\n\n", ReadBuffer);

	// Prepare scope for waveform

	ibwrt(Dev, "DAT:SOU CH1", 11L);
	ibwrt(Dev, "DAT:ENC RIB;WID 1", 17L);
	ibwrt(Dev, "HOR:RECORDL 500", 15L);
	ibwrt(Dev, "DAT:STAR 1", 10L);
	ibwrt(Dev, "DAT:STOP 500", 12L);
	ibwrt(Dev, "HEAD OFF", 8L);

	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to initialize waveform parameters");
		return 1;
	}

	ibwrt(Dev, "ACQ:STATE RUN", 13L);

	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to acquire waveform");
		return 1;
	}

	ibwrt(Dev, "CURV?", 5L);
	ibrd(Dev, &c, 1);
	ibrd(Dev, &c, 1);
	rslt = atoi(&c);
	ibrd(Dev, ReadBuffer, rslt);
	ReadBuffer[rslt] = '\0';
	rslt = atoi(ReadBuffer);
	ibrd(Dev, wfm, rslt);
	ibrd(Dev, &c, 1);

	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to read waveform");
		return 1;
	}

	ibwrt(Dev, "WFMPRE:CH1:NR_PT?;YOFF?;YMULT?;XINCR?;PT_OFF?;XUNIT?;YUNIT?", 59L);

	memset(ReadBuffer, 0, 80);

	ibrd(Dev, ReadBuffer, 80);

	if (ibsta & ERR)
	{
		d_GPIBCleanup(Dev, "Unable to read waveform preamble");
		return 1;
	}

	sscanf_s(ReadBuffer, "%d;%e;%e;%e;%d;%[^;];%s", &nr_pt, &yoff, &ymult, &xincr, &pt_off, xunit, yunit);

	//outfile = fopen("WFM_DATA.dat", "wb");

	//_strdate(date);
	//_strtime(timenow);

	//fprintf(outfile, "%s,%s,\"%s\",\"%s\",\"%s\"\n", xunit, yunit, date, timenow, "CH1");

	//for (i = 0; i<nr_pt; i++)
	//{
	//	fprintf(outfile, "%14.12f,%14.12f\n", (float)(i - pt_off)*(float)(xincr),
	//		(float)(((float)wfm[i] - (float)yoff) * ymult));
	//}

	//fclose(outfile);
	// Offline the board and end
	ibonl(GPIB0, 0);
	return 0;
	//// Ensure that the board being used is Controller-In-Charge
	//SendIFC(GPIB0);
	//if (ibsta & ERR)
	//{
	//	GPIBCleanup(GPIB0, "Unable to open board");
	//	return 1;
	//}

	//// Create an array containing valid GPIB primary addresses to
	//// pass to FindLstn, terminated by decl-32's NOADDR
	//for (loop = 0; loop < 30; loop++) {
	//	Instruments[loop] = (Addr4882_t)(loop + 1);
	//}
	//Instruments[30] = NOADDR;

	//// Find all instruments on the bus, store addresses in Result
	//printf("Finding all instruments on the bus...\n\n");

	//FindLstn(GPIB0, Instruments, Result, 31);
	//if (ibsta & ERR)
	//{
	//	GPIBCleanup(GPIB0, "Unable to issue FindLstn Call");
	//	return 1;
	//}

	//// Assign the number of isntruments found and print such a number
	//Num_Instruments = ibcntl;

	//printf("Number of instruments found = %d\n\n", Num_Instruments);

	//Result[Num_Instruments] = NOADDR;

	////Print out each instrument's PAD and SAD

	//for (loop = 0; loop < Num_Instruments; loop++)
	//{
	//	PAD = GetPAD(Result[loop]);
	//	SAD = GetSAD(Result[loop]);
	//	Primary_Addr[loop] = PAD;

	//	if (SAD == NO_SAD)
	//	{
	//		Secondary_Addr[loop] = 0;
	//		printf("The instrument at Result[%d]: PAD = %d SAD = NONE\n\n",
	//			loop, PAD, SAD);
	//	}
	//	else
	//	{
	//		Secondary_Addr[loop] = SAD;
	//		printf("The instrument at Result[%d]: PAD = %d SAD = %d\n\n",
	//			loop, PAD, SAD);
	//	}

	//}

	//// Assign each device an identifier, then clear the device

	//for (loop = 0; loop < Num_Instruments; loop++)
	//{
	//	Dev[loop] = ibdev(BDINDEX, Primary_Addr[loop], Secondary_Addr[loop],
	//		TIMEOUT, EOTMODE, EOSMODE);
	//	if (ibsta & ERR)
	//	{
	//		d_GPIBCleanup(Dev[loop], "Unable to open device");
	//		return 1;
	//	}

	//	ibclr(Dev[loop]);
	//	if (ibsta & ERR)
	//	{
	//		d_GPIBCleanup(Dev[loop], "Unable to clear device");
	//		return 1;
	//	}
	//}

	//// Communicate with each device
	//// Currently, only querying with "*IDN?"

	//printf("Querying each device with [*IDN?] ...\n\n");

	//for (loop = 0; loop < Num_Instruments; loop++)
	//{
	//	ibwrt(Dev[loop], "*IDN?", 5L);
	//	if (ibsta & ERR)
	//	{
	//		d_GPIBCleanup(Dev[loop], "Unable to write to device");
	//		return 1;
	//	}

	//	ibrd(Dev[loop], ReadBuffer, ARRAYSIZE);
	//	if (ibsta & ERR)
	//	{
	//		d_GPIBCleanup(Dev[loop], "Unable to read data from device");
	//		return 1;
	//	}

	//	ReadBuffer[ibcntl] = '\0';
	//	printf("Returned string from Result[%d]: %s\n\n", loop, ReadBuffer);
	//}

	//scanf_s("%c", &c);

	//// Offline the board and end
	//ibonl(GPIB0, 0);
	//return 0;
}
