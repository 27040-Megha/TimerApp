# Task Creation Test Plan

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
      <td rowspan="2"><strong>Add new Activity</strong></td>
      <td>Valid string</td>
      <td rowspan="2">Task Name (string)</td>
      <td><code>Chess</code></td>
      <td>SUCCESS: Task added successfully</td>
    </tr>
    <tr>
      <td>Invalid string</td>
      <td><code>null</code> or empty</td>
      <td>ERROR: Task name can't be null or empty</td>
    </tr>
  </tbody>
</table>
