package com.samsung.android.sdk.pass;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import com.samsung.android.sdk.SsdkInterface;
import com.samsung.android.sdk.SsdkUnsupportedException;
import com.samsung.android.sdk.SsdkVendorCheck;
import java.lang.reflect.Method;

public class Spass
  implements SsdkInterface
{
  public static final int DEVICE_FINGERPRINT = 0;
  public static final int DEVICE_FINGERPRINT_FINGER_INDEX = 1;
  public static final int DEVICE_FINGERPRINT_CUSTOMIZED_DIALOG = 2;
  public static final int DEVICE_FINGERPRINT_UNIQUE_ID = 3;
  public static final int DEVICE_FINGERPRINT_AVAILABLE_PASSWORD = 4;
  private SpassFingerprint a;
  
  public int getVersionCode()
  {
    return 8;
  }
  
  public String getVersionName()
  {
    return String.format("%d.%d.%d", new Object[] { Integer.valueOf(1), Integer.valueOf(2), Integer.valueOf(1) });
  }
  
  public void initialize(Context paramContext)
    throws SsdkUnsupportedException
  {

  }
  
  public boolean isFeatureEnabled(int paramInt)
  {

    throw new IllegalArgumentException("type passed is not valid");
  }
  

}