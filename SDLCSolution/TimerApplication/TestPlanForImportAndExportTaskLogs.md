# Export & Import Logs Test Plan

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
    <!-- Export Logs (5 rows) -->
    <tr>
      <td rowspan="5"><strong>Export Logs</strong></td>
      <td>Invalid Integer format</td>
      <td rowspan="5">Index</td>
      <td><code>ab, $, +</code></td>
      <td>ERROR: Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>-1 or any value below 0</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>above activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid index, no logs recorded for the activity</td>
      <td><code>1</code></td>
      <td>ERROR: No logs recorded for the activity to export</td>
    </tr>
    <tr>
      <td>Valid index, Activity logs recorded</td>
      <td><code>1</code></td>
      <td>SUCCESS: Logs were exported successfully</td>
    </tr>
    <!-- Import Logs (6 rows) -->
    <tr>
      <td rowspan="6"><strong>Import Logs</strong></td>
      <td>Index out of range, Invalid file path</td>
      <td rowspan="6">Index, File Path (string)</td>
      <td><code>-1, filename without .csv</code></td>
      <td>ERROR: Index out of range/ ERROR: Invalid file path</td>
    </tr>
    <tr>
      <td>Valid index, Valid file Path (But no activities found)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>ERROR: No activities found to import data</td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (But file doesn't exist)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>ERROR: File does not exist</td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (File exist but invalid CSV format)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>ERROR: Invalid CSV format</td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (Valid CSV data but overlapping existing entries)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>ERROR: Log entries overlap with the existing entries</td>
    </tr>
    <tr>
      <td>Valid index, Valid file path (Valid data with no overlapping entries)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>SUCCESS: Logs were imported successfully</td>
    </tr>
  </tbody>
</table>
