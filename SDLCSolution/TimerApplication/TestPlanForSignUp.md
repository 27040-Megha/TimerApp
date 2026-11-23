# Signup & Validation Test Plan

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
      <td rowspan="2"><strong>Signup</strong></td>
      <td>Valid Username, valid email, valid password (New email ID)</td>
      <td rowspan="2">User name, email, password</td>
      <td><code>Megha</code>, <code>meghaeg@gmail.com</code>, <code>Megha@123</code></td>
      <td>SUCCESS: Signup successful</td>
    </tr>
    <tr>
      <td>Valid Username, valid email, valid password (Email exists already)</td>
      <td><code>Megha</code>, <code>meghaeg@gmail.com</code>, <code>Megha@123</code></td>
      <td>ERROR: Email ID already exists</td>
    </tr>
    <tr>
      <td rowspan="6"><strong>Email validation</strong></td>
      <td>Valid Email</td>
      <td rowspan="6">email (string)</td>
      <td><code>meghaeg@gmail.com</code>, <code>megha@kec.edu</code></td>
      <td>SUCCESS: Valid Email</td>
    </tr>
    <tr>
      <td>Invalid email (Missing @)</td>
      <td><code>meghagmail.com</code></td>
      <td>ERROR: Invalid Email format</td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters before @)</td>
      <td><code>@meghagmail.com</code></td>
      <td>ERROR: Invalid Email format</td>
    </tr>
    <tr>
      <td>Invalid email (Missing .)</td>
      <td><code>megha@gmailcom</code></td>
      <td>ERROR: Invalid Email format</td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters between @ and .)</td>
      <td><code>megha@.com</code></td>
      <td>ERROR: Invalid Email format</td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters after .)</td>
      <td><code>megha@gmail.</code></td>
      <td>ERROR: Invalid Email format</td>
    </tr>
    <tr>
      <td rowspan="5"><strong>Password validation</strong></td>
      <td>Valid Password</td>
      <td rowspan="5">Password (string)</td>
      <td><code>Megha@123</code></td>
      <td>SUCCESS: Valid Password</td>
    </tr>
    <tr>
      <td>Invalid Password (Length less than 8)</td>
      <td><code>Meg@12</code></td>
      <td>ERROR: Invalid Password format</td>
    </tr>
    <tr>
      <td>Invalid Password (Missing capital letter)</td>
      <td><code>megha@123</code></td>
      <td>ERROR: Invalid Password format</td>
    </tr>
    <tr>
      <td>Invalid Password (Missing Special character)</td>
      <td><code>megha1234</code></td>
      <td>ERROR: Invalid Password format</td>
    </tr>
    <tr>
      <td>Invalid Password (Missing numbers)</td>
      <td><code>meghaa@eg</code></td>
      <td>ERROR: Invalid Password format</td>
    </tr>
  </tbody>
</table>
