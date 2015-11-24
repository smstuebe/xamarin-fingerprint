using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware.Fingerprints;
using Android.OS;
using Java.Lang;
using SMS.Fingerprint.Abstractions;

namespace SMS.Fingerprint
{
    public class FingerprintImplementation : IFingerprint
    {
        private readonly FingerprintManager _fpService;
        public bool IsAvailable { get; private set; }

        public FingerprintImplementation()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                return;

            var context = Application.Context;
            if (context.CheckCallingPermission(Manifest.Permission.UseFingerprint) == Permission.Denied)
                return;

            _fpService = (FingerprintManager)context.GetSystemService(Class.FromType(typeof(FingerprintManager)));
            if (!_fpService.IsHardwareDetected)
                return;

            if (!_fpService.HasEnrolledFingerprints)
                return;
            
            IsAvailable = true;
        }


        public Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason)
        {
            return AuthenticateAsync(reason, new CancellationToken());
        }

        public async Task<FingerprintAuthenticationResult> AuthenticateAsync(string reason, CancellationToken cancellationToken)
        {
            if (!IsAvailable)
            {
                return new FingerprintAuthenticationResult { Status = FingerprintAuthenticationResultStatus.NotAvailable };
            }

            var cancellationSignal = new CancellationSignal();
            var callback = new FingerprintAuthenticationCallback();
            cancellationToken.Register(() => cancellationSignal.Cancel());

            _fpService.Authenticate(null, cancellationSignal, FingerprintAuthenticationFlags.None, callback, null);

            return await callback.GetTask();
        }
    }

    public class FingerprintDialogFragment : DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RetainInstance = true;
            SetStyle(DialogFragmentStyle.Normal, 0);        
        }

        //@Override
        //public View onCreateView(LayoutInflater inflater, ViewGroup container,
        //        Bundle savedInstanceState) {
        //    getDialog().setTitle(getString(R.string.sign_in));
        //    View v = inflater.inflate(R.layout.fingerprint_dialog_container, container, false);
        //    mCancelButton = (Button) v.findViewById(R.id.cancel_button);
        //    mCancelButton.setOnClickListener(new View.OnClickListener() {
        //        @Override
        //        public void onClick(View view) {
        //            dismiss();
        //        }
        //    });

        //    mSecondDialogButton = (Button) v.findViewById(R.id.second_dialog_button);
        //    mSecondDialogButton.setOnClickListener(new View.OnClickListener() {
        //        @Override
        //        public void onClick(View view) {
        //            if (mStage == Stage.FINGERPRINT) {
        //                goToBackup();
        //            } else {
        //                verifyPassword();
        //            }
        //        }
        //    });
        //    mFingerprintContent = v.findViewById(R.id.fingerprint_container);
        //    mBackupContent = v.findViewById(R.id.backup_container);
        //    mPassword = (EditText) v.findViewById(R.id.password);
        //    mPassword.setOnEditorActionListener(this);
        //    mPasswordDescriptionTextView = (TextView) v.findViewById(R.id.password_description);
        //    mUseFingerprintFutureCheckBox = (CheckBox)
        //            v.findViewById(R.id.use_fingerprint_in_future_check);
        //    mNewFingerprintEnrolledTextView = (TextView)
        //            v.findViewById(R.id.new_fingerprint_enrolled_description);
        //    mFingerprintUiHelper = mFingerprintUiHelperBuilder.build(
        //            (ImageView) v.findViewById(R.id.fingerprint_icon),
        //            (TextView) v.findViewById(R.id.fingerprint_status), this);
        //    updateStage();

        //    // If fingerprint authentication is not available, switch immediately to the backup
        //    // (password) screen.
        //    if (!mFingerprintUiHelper.isFingerprintAuthAvailable()) {
        //        goToBackup();
        //    }
        //    return v;
        //}

        public override void OnResume()
        {
            base.OnResume();
            //if (mStage == Stage.FINGERPRINT) {
            //    mFingerprintUiHelper.startListening(mCryptoObject);
            //}
        }

        public override void OnPause()
        {
            base.OnPause();
            //mFingerprintUiHelper.stopListening();
        }

        //@Override
        //public void onAttach(Activity activity) {
        //    super.onAttach(activity);
        //    mActivity = (MainActivity) activity;
        //}

        ///**
        // * Sets the crypto object to be passed in when authenticating with fingerprint.
        // */
        //public void setCryptoObject(FingerprintManager.CryptoObject cryptoObject) {
        //    mCryptoObject = cryptoObject;
        //}

        ///**
        // * Switches to backup (password) screen. This either can happen when fingerprint is not
        // * available or the user chooses to use the password authentication method by pressing the
        // * button. This can also happen when the user had too many fingerprint attempts.
        // */
        //private void goToBackup() {
        //    mStage = Stage.PASSWORD;
        //    updateStage();
        //    mPassword.requestFocus();

        //    // Show the keyboard.
        //    mPassword.postDelayed(mShowKeyboardRunnable, 500);

        //    // Fingerprint is not used anymore. Stop listening for it.
        //    mFingerprintUiHelper.stopListening();
        //}

        ///**
        // * Checks whether the current entered password is correct, and dismisses the the dialog and
        // * let's the activity know about the result.
        // */
        //private void verifyPassword() {
        //    if (!checkPassword(mPassword.getText().toString())) {
        //        return;
        //    }
        //    if (mStage == Stage.NEW_FINGERPRINT_ENROLLED) {
        //        SharedPreferences.Editor editor = mSharedPreferences.edit();
        //        editor.putBoolean(getString(R.string.use_fingerprint_to_authenticate_key),
        //                mUseFingerprintFutureCheckBox.isChecked());
        //        editor.apply();

        //        if (mUseFingerprintFutureCheckBox.isChecked()) {
        //            // Re-create the key so that fingerprints including new ones are validated.
        //            mActivity.createKey();
        //            mStage = Stage.FINGERPRINT;
        //        }
        //    }
        //    mPassword.setText("");
        //    mActivity.onPurchased(false /* without Fingerprint */);
        //    dismiss();
        //}

        ///**
        // * @return true if {@code password} is correct, false otherwise
        // */
        //private boolean checkPassword(String password) {
        //    // Assume the password is always correct.
        //    // In the real world situation, the password needs to be verified in the server side.
        //    return password.length() > 0;
        //}

        //private final Runnable mShowKeyboardRunnable = new Runnable() {
        //    @Override
        //    public void run() {
        //        mInputMethodManager.showSoftInput(mPassword, 0);
        //    }
        //};

        //private void updateStage() {
        //    switch (mStage) {
        //        case FINGERPRINT:
        //            mCancelButton.setText(R.string.cancel);
        //            mSecondDialogButton.setText(R.string.use_password);
        //            mFingerprintContent.setVisibility(View.VISIBLE);
        //            mBackupContent.setVisibility(View.GONE);
        //            break;
        //        case NEW_FINGERPRINT_ENROLLED:
        //            // Intentional fall through
        //        case PASSWORD:
        //            mCancelButton.setText(R.string.cancel);
        //            mSecondDialogButton.setText(R.string.ok);
        //            mFingerprintContent.setVisibility(View.GONE);
        //            mBackupContent.setVisibility(View.VISIBLE);
        //            if (mStage == Stage.NEW_FINGERPRINT_ENROLLED) {
        //                mPasswordDescriptionTextView.setVisibility(View.GONE);
        //                mNewFingerprintEnrolledTextView.setVisibility(View.VISIBLE);
        //                mUseFingerprintFutureCheckBox.setVisibility(View.VISIBLE);
        //            }
        //            break;
        //    }
        //}

        //@Override
        //public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
        //    if (actionId == EditorInfo.IME_ACTION_GO) {
        //        verifyPassword();
        //        return true;
        //    }
        //    return false;
        //}

        //@Override
        //public void onAuthenticated() {
        //    // Callback from FingerprintUiHelper. Let the activity know that authentication was
        //    // successful.
        //    mActivity.onPurchased(true /* withFingerprint */);
        //    dismiss();
        //}

        //@Override
        //public void onError() {
        //    goToBackup();
        //}

    }
}
