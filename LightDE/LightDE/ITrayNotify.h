/*  ************************************************************************  *
*				   itraynot.h				      *
*  ************************************************************************  */

/*  This header exists only for a rough definition of the ITrayNotify
interface, as implemented by EXPLORER in Windows XP and higher, but not
documented by Microsoft.

Copyright (C) 2008. Geoff Chappell. All rights reserved.  
Modified by Hianz.
*/

/*  ========================================================================  */

#pragma     once


#include <windows.h>
#include <objbase.h>
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <commctrl.h>
#include <iostream>

#include "Cominit.h"
#include <wchar.h>
using namespace std;

/*  ************************************************************************  */
/*  Forward references	*/

typedef interface ITrayNotify ITrayNotify;
typedef interface INotificationCB INotificationCB;

/*  ************************************************************************  */

typedef struct tagNOTIFYITEM
{
	PWSTR pszExeName;
	PWSTR pszTip;
	HICON hIcon;
	HWND hWnd;
	DWORD dwPreference;
	UINT uID;
	GUID guidItem;
} NOTIFYITEM;

//INotificationCB GUID
[
	uuid ("D782CCBA-AFB0-43F1-94DB-FDA3779EACCB")
]

interface INotificationCB : public IUnknown
{
	virtual HRESULT __stdcall Notify (ULONG, NOTIFYITEM *) = 0;
};

//ItrayNotify GUID
[
	uuid ("FB852B2C-6BAD-4605-9551-F15F87830935")
]

//Virtual Functions for Windows Xp and higher
interface ITrayNotify : public IUnknown
{
	virtual HRESULT __stdcall RegisterCallback (INotificationCB *) = 0;

	virtual HRESULT __stdcall SetPreference (NOTIFYITEM const *) = 0;

	virtual HRESULT __stdcall EnableAutoTray (BOOL) = 0;
};

/*
Virtual Functions for Windows 8
interface ITrayNotify : public IUnknown
{
	virtual HRESULT __stdcall RegisterCallback (INotificationCB *,unsigned long *) = 0;

	virtual HRESULT __stdcall UnregisterCallback (unsigned long *) = 0;

	virtual HRESULT __stdcall SetPreference (NOTIFYITEM const *) = 0;

	virtual HRESULT __stdcall EnableAutoTray (BOOL) = 0;

	//Look at virtual function table for correct signature
	virtual HRESULT __stdcall DoAction (BOOL) = 0;
};
*/
//TrayNotifyClass GUID
[
	uuid ("25DEAD04-1EAC-4911-9E3A-AD0A4AB560FD")
]
class TrayNotify : public ITrayNotify
{
};

/*  ************************************************************************  */