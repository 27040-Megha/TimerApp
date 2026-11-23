# Timer Activities Test Plan

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
      <td rowspan="5"><strong>Start Timer</strong></td>
      <td>Index out of range</td>
      <td rowspan="5">Index</td>
      <td><code>-1</code> or any value below 0</td>
      <td>ERROR: Argument out of range exception</td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td><code>ab</code>, <code>$</code>, <code>+</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td>above <code>activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status - Running</td>
      <td><code>1</code></td>
      <td>ERROR: Can't Start Timer, Timer is actively running</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status - Not Started/ Stopped</td>
      <td><code>2</code></td>
      <td>SUCCESS: Timer started successfully</td>
    </tr>
    <tr>
      <td rowspan="4"><strong>Stop Timer</strong><br><em>(Only tasks with active timers will be displayed)</em></td>
      <td>Index out of range</td>
      <td rowspan="4">Index</td>
      <td><code>-1</code> or any value below 0</td>
      <td>ERROR: Argument out of range exception</td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td><code>ab</code>, <code>$</code>, <code>+</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td>above <code>activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid Index</td>
      <td><code>2</code></td>
      <td>SUCCESS: Timer stopped successfully</td>
    </tr>
  </tbody>
</table>
