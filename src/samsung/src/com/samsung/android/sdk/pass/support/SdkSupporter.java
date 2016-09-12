package com.samsung.android.sdk.pass.support;

import android.util.Log;
import java.lang.reflect.Field;
import java.util.HashMap;
import java.util.Map;

public class SdkSupporter
{
  public static final String CLASSNAME_FINGERPRINT_MANAGER = "com.samsung.android.fingerprint.FingerprintManager";
  public static final String CLASSNAME_FINGERPRINT_CLIENT = "com.samsung.android.fingerprint.IFingerprintClient$Stub";
  public static final String CLASSNAME_FINGERPRINT_CLIENT_SPEC_BUILDER = "com.samsung.android.fingerprint.FingerprintManager$FingerprintClientSpecBuilder";
  
  public static boolean copyStaticFields(Object paramObject, Class paramClass, String paramString1, String paramString2)
  {

    return true;
  }
}