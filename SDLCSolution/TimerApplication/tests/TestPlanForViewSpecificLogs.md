# View Specific Activity Logs Test Plan

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
      <td rowspan="4"><strong>View Specific Activity logs</strong></td>
      <td>Index out of range</td>
      <td rowspan="4">Index</td>
      <td><code>-1 or any value below 0</code></td>
      <td>ERROR: ArgumentOutOfRangeException - Index out of range</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>above activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid index, No logs recorded for the activity</td>
      <td><code>2</code></td>
      <td>ERROR: No activity logs recorded for the activity</td>
    </tr>
    <tr>
      <td>Valid index, Activity logs recorded</td>
      <td><code>2</code></td>
      <td>SUCCESS: Display activity logs</td>
    </tr>
  </tbody>
</table>
