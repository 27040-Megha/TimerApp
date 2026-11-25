# Login Test Plan

<table>
  <thead>
    <tr>
      <th>Feature</th>
      <th>Scenario / Description</th>
      <th>Input Parameters</th>
      <th>Test Data</th>
      <th>Expected Result</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td rowspan="3"><strong>Login</strong></td>
      <td>Valid email, Valid Password (Account with this email does not exist)</td>
      <td rowspan="3">Email, password</td>
      <td><code>meghaeg@gmail.com</code>, <code>Megha@123</code></td>
      <td>ERROR: Account does not exists</td>
    </tr>
    <tr>
      <td>Valid email, Valid Password (Credentials do not match)</td>
      <td><code>meghaeg@gmail.com</code>, <code>Megha@123</code></td>
      <td>ERROR: Credentials does not match</td>
    </tr>
    <tr>
      <td>Valid email, Valid Password (Credentials match)</td>
      <td><code>meghaeg@gmail.com</code>, <code>Megha@123</code></td>
      <td>SUCCESS: Login Successful</td>
    </tr>
  </tbody>
</table>
