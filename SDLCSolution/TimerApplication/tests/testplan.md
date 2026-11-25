# Time Logger App — Complete Test Plan

<table>
  <thead>
    <tr>
      <th>Feature</th>
      <th>Scenario</th>
      <th>Parameter</th>
      <th>Test Case</th>
      <th>Expected Output</th>
      <th>Actual Result</th>
      <th>Test Status</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td rowspan="6"><strong>Delete Activity</strong></td>
      <td>No activities exist</td>
      <td>Index</td>
      <td>2</td>
      <td>No activities found to delete!</td>
      <td>No activities found to delete!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td></td>
      <td>ab, $, +</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Running</td>
      <td></td>
      <td>1</td>
      <td>Active timer running, Can&#x27;t perform Operation</td>
      <td>Active timer running, Can&#x27;t perform Operation</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Stopped</td>
      <td></td>
      <td>2</td>
      <td>Task Deleted Successfully!</td>
      <td>Task Deleted Successfully!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="7"><strong>Edit Activity</strong></td>
      <td>No activities exist</td>
      <td>Index, Task Name (string)</td>
      <td>1, Chess</td>
      <td>No activities found to edit!</td>
      <td>No activities found to edit!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range, valid string name</td>
      <td></td>
      <td>-1, Chess</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range, valid string name</td>
      <td></td>
      <td>above activities.Count, Chess</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range, Invalid string name</td>
      <td></td>
      <td>-1 or above activities.Count, null or empty</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td></td>
      <td>ab or $ or +, null or empty or valid string</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, invalid string name</td>
      <td></td>
      <td>2, null or empty</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, valid string name</td>
      <td></td>
      <td>2, chess</td>
      <td>Task Added Successfully!</td>
      <td>Task Added Successfully!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="2"><strong>Add new Activity</strong></td>
      <td>Valid string</td>
      <td>Task Name (string)</td>
      <td>Chess</td>
      <td>SUCCESS: Task added successfully</td>
      <td>SUCCESS: Task added successfully</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid string</td>
      <td></td>
      <td>null or empty</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="5"><strong>Start Timer</strong></td>
      <td>Index out of range</td>
      <td>Index</td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td></td>
      <td>ab, $, +</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Running</td>
      <td></td>
      <td>1</td>
      <td>Can&#x27;t Start, Timer is already running for this task</td>
      <td>Can&#x27;t Start, Timer is already running for this task</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Not Started/ Stopped</td>
      <td></td>
      <td>2</td>
      <td>Timer started successfully!</td>
      <td>Timer started successfully!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="4"><strong>Stop Timer (Only tasks with active timers will be displayed)</strong></td>
      <td>Index out of range</td>
      <td>Index</td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td></td>
      <td>ab, $, +</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid Index</td>
      <td></td>
      <td>2</td>
      <td>Timer stopped successfully</td>
      <td>Timer stopped successfully</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="4"><strong>View Specific Activity logs</strong></td>
      <td>Index out of range</td>
      <td>Index</td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, No logs recorded for the activity</td>
      <td></td>
      <td>2</td>
      <td>No activity logs recorded for the activity</td>
      <td>No activity logs recorded for the activity</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Activity logs recorded</td>
      <td></td>
      <td>2</td>
      <td>Logs will be displayed</td>
      <td>chess  Duration: 00:03:47<br>1. swim1 (Duration: 00:03:47)<br>   Start: 19-11-2026 16:20:52,  End: 19-11-2026 16:24:39<br>2. chess-20261125-222533<br>   Start: 25-11-2026 22:25:33,  End: Running</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="6"><strong>Edit activity log</strong></td>
      <td>Invalid integer format, valid/invalid string</td>
      <td>Index, Task log name (string)</td>
      <td>ab or $ or +, null or empty or valid string</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range, Invalid string name</td>
      <td></td>
      <td>-1, null or empty</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range, Valid string name</td>
      <td></td>
      <td>above activities.Count, Chess-Log12</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, invalid string name</td>
      <td></td>
      <td>2, null or empty</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td>Invalid string format, Task name should not be null or empty!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, valid string name (Duplicate name)</td>
      <td></td>
      <td>2, Chess-Log1</td>
      <td>A log with this name already exists for this task!</td>
      <td>A log with this name already exists for this task!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, valid string name</td>
      <td></td>
      <td>2, Chess-NewLog</td>
      <td>Edit successful!</td>
      <td>Edit successful!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="6"><strong>Delete activity log</strong></td>
      <td>No activity log exist</td>
      <td>Index</td>
      <td>2</td>
      <td>No activity logs found to delete!</td>
      <td>No activity logs found to delete!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td></td>
      <td>ab, $, +</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Running</td>
      <td></td>
      <td>1</td>
      <td>Cannot delete a log while running timer! Stop the timer first</td>
      <td>Cannot delete a log while running timer! Stop the timer first</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Stopped</td>
      <td></td>
      <td>1</td>
      <td>Activity Log Deleted Successfully!</td>
      <td>Activity Log Deleted Successfully!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="5"><strong>Export Logs</strong></td>
      <td>Invalid Integer format</td>
      <td>Index</td>
      <td>ab, $, +</td>
      <td>Invalid integer format</td>
      <td>Invalid integer format</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>-1 or any value below 0</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td></td>
      <td>above activities.Count</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, no logs recorded for the activity</td>
      <td></td>
      <td>1</td>
      <td>No completed logs recorded for the activity to export</td>
      <td>No completed logs recorded for the activity to export</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Activity logs recorded</td>
      <td></td>
      <td>1</td>
      <td>Logs exported successfully to {savedPath}</td>
      <td>Logs exported successfully to {savedPath}</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="6"><strong>Import Logs</strong></td>
      <td>Index out of range, Valid file path</td>
      <td>Index, File Path (string)</td>
      <td>-1, Chess-Log.csv</td>
      <td>Index Out of Range</td>
      <td>Index Out of Range</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Valid file Path (But no activities found)</td>
      <td></td>
      <td>1, ChessActivity.csv</td>
      <td>No activity found to import time log</td>
      <td>No activity found to import time log</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (But file doesn&#x27;t exist)</td>
      <td></td>
      <td>1, ChessActivity.csv</td>
      <td>The specified file was not found at {location}</td>
      <td>The specified file was not found at C:\Users\megha.easwaramoorthy\Downloads\Cricket-25-11-2026_17-45-57.csv</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Invalid file path</td>
      <td></td>
      <td>1, ChessActivity.json</td>
      <td>Invalid file path, give a valid .csv file path</td>
      <td>Invalid file path, give a valid .csv file path</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (Valid CSV data but overlapping existing entries)</td>
      <td></td>
      <td>1, ChessActivity.csv</td>
      <td>No logs were imported because time overlaps with existing activity logs or timestamps are invalid</td>
      <td>No logs were imported because time overlaps with existing activity logs or timestamps are invalid</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (Valid data with no overlapping entries)</td>
      <td></td>
      <td>1, ChessActivity.csv</td>
      <td>Logs imported successfully!</td>
      <td>Logs imported successfully!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="2"><strong>Signup</strong></td>
      <td>Valid Username, valid email, valid password (New email ID)</td>
      <td>User name, email, password</td>
      <td>Megha, meghaeg@gmail.com, Megha@123</td>
      <td>Signup Successful!</td>
      <td>Signup Successful!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid Username, valid email, valid password (Email exists already)</td>
      <td></td>
      <td>Megha, meghaeg@gmail.com, Megha@123</td>
      <td>Signup failed: Account already registered with same email ID</td>
      <td>Signup failed: Account already registered with same email ID</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="6"><strong>Email validation</strong></td>
      <td>Valid Email</td>
      <td>email (string)</td>
      <td>meghaeg@gmail.com, megha@kec.edu</td>
      <td>Valid email accepted</td>
      <td>Valid email accepted</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid email (Missing @)</td>
      <td></td>
      <td>meghagmail.com</td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters before @)</td>
      <td></td>
      <td>@meghagmail.com</td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid email (Missing .)</td>
      <td></td>
      <td>megha@gmailcom</td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters between @ and .)</td>
      <td></td>
      <td>megha@.com</td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid email (Missing characters after .)</td>
      <td></td>
      <td>megha@gmail.</td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="5"><strong>Password validation</strong></td>
      <td>Valid Password</td>
      <td>Password (string)</td>
      <td>Megha@123</td>
      <td>Valid password accepted</td>
      <td>Valid password accepted</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Password (Length less than 8)</td>
      <td></td>
      <td>Meg@12</td>
      <td>The password should atleast be of length 8</td>
      <td>The password should atleast be of length 8</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Password (Missing capital letter)</td>
      <td></td>
      <td>megha@123</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Password (Missing Special character)</td>
      <td></td>
      <td>megha1234</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Invalid Password (Missing numbers)</td>
      <td></td>
      <td>meghaa@eg</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td>The password should contain atleast 1 Uppercase, 1 Lowercase and 1 Special Character</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td rowspan="4"><strong>Login</strong></td>
      <td>Invalid email, Invalid password</td>
      <td>Email, password</td>
      <td></td>
      <td>Invalid email format!</td>
      <td>Invalid email format!</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid email, Valid Password (Account with this email does not exist)</td>
      <td></td>
      <td>meghaeg@gmail.com, Megha@123</td>
      <td>Account doesn&#x27;t exist, Signup first</td>
      <td>Account doesn&#x27;t exist, Signup first</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid email, Valid Password (Credentails does not match)</td>
      <td></td>
      <td>meghaeg@gmail.com, Megha@123</td>
      <td>Invalid credentials, Try logging in again</td>
      <td>Invalid credentials, Try logging in again</td>
      <td><strong>PASS</strong></td>
    </tr>
    <tr>
      <td>Valid email, Valid Password (Credentails match)</td>
      <td></td>
      <td>meghaeg@gmail.com, Megha@123</td>
      <td>Login Successful</td>
      <td>Login Successful</td>
      <td><strong>PASS</strong></td>
    </tr>
  </tbody>
</table>
