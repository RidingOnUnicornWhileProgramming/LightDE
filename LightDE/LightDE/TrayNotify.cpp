/*  ************************************************************************  *
*				  TrayNotify.cpp				      *
*  ************************************************************************  */
#include "Cominit.h"
#include "ITrayNotify.h"
using namespace std;

typedef void (__stdcall *PCallBack)(NOTIFYITEM *item);
PCallBack callbackTemp;
NOTIFYITEM *globalItem;

/* Interface declaration */
ITrayNotify *m_ITrayNotify;


extern "C" __declspec(dllexport) void __stdcall PerformActionWithCallBack(PCallBack pCallBack)
{
	if (pCallBack)
	{
		callbackTemp = pCallBack;
		pCallBack(globalItem);
	}
}

class Notifications : public INotificationCB
{
	/*	INotificationCB methods  */
	HRESULT __stdcall QueryInterface (REFIID, PVOID *);
	ULONG __stdcall AddRef (VOID);
	ULONG __stdcall Release (VOID);
	__declspec(dllexport) HRESULT __stdcall Notify (ULONG, NOTIFYITEM *);

public:
	BOOL EnableNotifications (BOOL);

	Notifications()
	{
		m_ITrayNotify = NULL;
	};
};
Notifications *notifications;
/*  ========================================================================  */
/*  INotificationCB methods  */
HRESULT _stdcall Notifications :: QueryInterface (REFIID riid,PVOID *ppv)
{
	if (ppv == NULL) return E_POINTER;

	if (riid == __uuidof (INotificationCB)) {
		*ppv = (INotificationCB *) this;
	}
	else if (riid == IID_IUnknown) {
		*ppv = (IUnknown *) this;
	}
	else {
		return E_NOINTERFACE;
	}

	((IUnknown *) *ppv) -> AddRef ();
	return S_OK;
}

ULONG __stdcall Notifications :: AddRef (VOID)
{
	return 1;
}

ULONG __stdcall Notifications :: Release (VOID)
{
	return 1;
}
HRESULT _stdcall Notifications :: Notify (
	ULONG Event,
	NOTIFYITEM *NotifyItem)
{
	globalItem = NotifyItem;
	PerformActionWithCallBack(callbackTemp);
	return S_OK;
}
/*  ------------------------------------------------------------------------  */
/*  Enabling notifications  */

BOOL Notifications :: EnableNotifications (BOOL bEnable)
{
	HRESULT hr;

	if (bEnable && m_ITrayNotify == NULL) {

		hr = CoCreateInstance (
			__uuidof (TrayNotify),
			NULL,
			CLSCTX_LOCAL_SERVER,
			__uuidof (ITrayNotify),
			(PVOID *) &m_ITrayNotify);

		if (FAILED (hr)) {
			//PutError (NULL, L"Error 0x%08X obtaining ITrayNotify", hr);
			return FALSE;
		}
	}

	if (m_ITrayNotify == NULL) return FALSE;


	hr = m_ITrayNotify -> RegisterCallback (bEnable ? this : NULL);

	if (FAILED (hr)) {
		//PutError (NULL, L"Error 0x%08X registering callback", hr);
	}

	if (!bEnable) {
		m_ITrayNotify -> Release ();
		m_ITrayNotify = NULL;
	}

	return SUCCEEDED (hr);
}

extern "C" __declspec(dllexport) void __stdcall EnableAutoTrayFunction(bool enabled)
{
	if (m_ITrayNotify != NULL)
		m_ITrayNotify->EnableAutoTray(enabled);
}

extern "C" __declspec(dllexport) void __stdcall SetPreferenceFunction(NOTIFYITEM *item)
{
	if (m_ITrayNotify != NULL)
		m_ITrayNotify->SetPreference(item);
}

//void main()
//{
//	for(;;){
//	Notifications *notifications = new Notifications;
//		notifications -> EnableNotifications(TRUE);
//		MSG msg;
//		int result = GetMessage (&msg, NULL, 0, 0);
//	}
//
//}
extern "C" __declspec(dllexport) void __stdcall Begin()
{
	//Notifications *notifications;
	 notifications = new Notifications;
		notifications -> EnableNotifications(TRUE);

	//notifications -> EnableNotifications(FALSE);
}

extern "C" __declspec(dllexport) void __stdcall End()
{
	notifications -> EnableNotifications(FALSE);
}

extern "C" __declspec(dllexport) int a(int b)
{
      return b;
}


