package com.samsung.android.sdk.pass;

public class SpassInvalidStateException
  extends IllegalStateException
{
  public static final int STATUS_OPERATION_DENIED = 1;
  private int a = 0;
  
  public SpassInvalidStateException(String paramString, int paramInt)
  {
    super(paramString);
    this.a = paramInt;
  }
  
  public int getType()
  {
    return this.a;
  }
}