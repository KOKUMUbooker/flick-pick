## Sign Up Flow
1. UI sends details & if valid user sends server sends a verification email to user
   - Once email is sent, server sends succes msg & emailVerification token & UI navigates to /verify-email?tkn=${res.emailVerificationToken}
  
2. User checks his email, clicks on verification link which makes GET /verify-email?tkn=<emailToken> on the server which validates token & if valid redirects user to client on route "/email-verified?email={}" to show success message. 
    -  Link to login should navigate to login with the email & prefill the email field with it
3. User can then proceed to login again

## Login Flow
1. UI sends login details to server and if everythings fine sends success msg to UI which routes to /dashboard
    - [TODO] : If error for email verification is sent, navigate to email-verified?from="log" page
    - If any other error UI should just display the error message to user

## Forgot password Flow
1. User enters email that he should get the password reset link containing a token & sends it to server which will send email to it if it exists
2. User goes to their email clicks on the link & gets taken to the /reset-password page on the UI where, he provides new & old password then submits.
   - If everything goes on well, take user to login page