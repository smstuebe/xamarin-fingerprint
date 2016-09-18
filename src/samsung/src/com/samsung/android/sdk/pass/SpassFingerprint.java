package com.samsung.android.sdk.pass;

import android.content.Context;
import android.util.SparseArray;

import java.util.ArrayList;

public class SpassFingerprint
{
  public static final int STATUS_AUTHENTIFICATION_SUCCESS = 0;
  public static final int STATUS_TIMEOUT_FAILED = 4;
  public static final int STATUS_SENSOR_FAILED = 7;
  public static final int STATUS_USER_CANCELLED = 8;
  public static final int STATUS_BUTTON_PRESSED = 9;
  public static final int STATUS_QUALITY_FAILED = 12;
  public static final int STATUS_USER_CANCELLED_BY_TOUCH_OUTSIDE = 13;
  public static final int STATUS_AUTHENTIFICATION_FAILED = 16;
  public static final int STATUS_OPERATION_DENIED = 51;
  public static final int STATUS_AUTHENTIFICATION_PASSWORD_SUCCESS = 100;
  public static final String ACTION_FINGERPRINT_RESET = "com.samsung.android.intent.action.FINGERPRINT_RESET";
  public static final String ACTION_FINGERPRINT_REMOVED = "com.samsung.android.intent.action.FINGERPRINT_REMOVED";
  public static final String ACTION_FINGERPRINT_ADDED = "com.samsung.android.intent.action.FINGERPRINT_ADDED";

  public SpassFingerprint(Context paramContext)
  {

  }

  public void cancelIdentify()
  {

  }
  
  public void startIdentify(IdentifyListener paramIdentifyListener)
  {

  }
  
  public void startIdentifyWithDialog(Context paramContext, IdentifyListener paramIdentifyListener, boolean paramBoolean)
  {

  }

  public boolean hasRegisteredFinger()
  {
    return true;
  }
  
  public int getIdentifiedFingerprintIndex()
  {
    return 0;
  }
  
  public SparseArray getRegisteredFingerprintName()
  {
    return null;
  }
  
  public SparseArray getRegisteredFingerprintUniqueID()
  {
    return null;
  }

  public void setIntendedFingerprintIndex(ArrayList paramArrayList)
  {

  }
  
  public void setDialogTitle(String paramString, int paramInt)
  {

  }
  
  public void setDialogIcon(String paramString)
  {

  }
  

  public void setDialogBgTransparency(int paramInt)
  {

  }
  
  public void setCanceledOnTouchOutside(boolean paramBoolean)
  {

  }
  
  public void setDialogButton(String paramString)
  {

  }
  
  public void changeStandbyString(String paramString)
  {

  }
  
  public String getGuideForPoorQuality()
  {
    return "";
  }

  public static abstract interface IdentifyListener
  {
    public abstract void onFinished(int paramInt);
    
    public abstract void onReady();
    
    public abstract void onStarted();
    
    public abstract void onCompleted();
  }
  
  public static abstract interface RegisterListener
  {
    public abstract void onFinished();
  }
}