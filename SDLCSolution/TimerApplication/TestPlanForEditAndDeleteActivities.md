# Delete and Edit Features Test Plan

<table>
  <thead>
    <tr>
      <th>Feature</th>
      <th>Scenario</th>
      <th>Parameter</th>
      <th>Test Case</th>
      <th>Expected Output</th>
    </tr>
  </thead>
  <tbody>
    <!-- Delete Activity (6 rows) -->
    <tr>
      <td rowspan="6"><strong>Delete Activity</strong></td>
      <td>No activities exist</td>
      <td rowspan="6">Index</td>
      <td><code>2</code></td>
      <td>ERROR: No activities found to delete</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>-1 or any value below 0</code></td>
      <td>ERROR: Argument out of range Exception</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>above activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td><code>ab, $, +</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Running</td>
      <td><code>1</code></td>
      <td>ERROR: Can't delete activity, Timer is running currently</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Stopped</td>
      <td><code>2</code></td>
      <td>SUCCESS: Activity Deleted successfully</td>
    </tr>
    <!-- Edit Activity (7 rows) -->
    <tr>
      <td rowspan="7"><strong>Edit Activity</strong></td>
      <td>No activities exist</td>
      <td rowspan="7">Index, Task Name (string)</td>
      <td><code>1, Chess</code></td>
      <td>ERROR: No activities found to edit</td>
    </tr>
    <tr>
      <td>Index out of range, valid string name</td>
      <td><code>-1, Chess</code></td>
      <td>ERROR: Argument out of range Exception</td>
    </tr>
    <tr>
      <td>Index out of range, valid string name</td>
      <td><code>above activities.Count, Chess</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Index out of range, Invalid string name</td>
      <td><code>-1 or above activities.Count, null or empty</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td><code>ab or $ or +, null or empty or valid string</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Valid index, invalid string name</td>
      <td><code>2, null or empty</code></td>
      <td>ERROR: String can't be null or empty</td>
    </tr>
    <tr>
      <td>Valid index, valid string name</td>
      <td><code>2, chess</code></td>
      <td>SUCCESS: Task name edited successfully</td>
    </tr>
    <!-- Edit activity log (6 rows) -->
    <tr>
      <td rowspan="6"><strong>Edit activity log</strong></td>
      <td>Invalid integer format, valid/invalid string</td>
      <td rowspan="6">Index, Task log name (string)</td>
      <td><code>ab or $ or +, null or empty or valid string</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range, Invalid string name</td>
      <td><code>-1, null or empty</code></td>
      <td>ERROR: Argument out of range Exception</td>
    </tr>
    <tr>
      <td>Index out of range, Valid string name</td>
      <td><code>above activities.Count, Chess-Log12</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid index, invalid string name</td>
      <td><code>2, null or empty</code></td>
      <td>ERROR: Invalid String</td>
    </tr>
    <tr>
      <td>Valid index, valid string name (Duplicate name)</td>
      <td><code>2, Chess-Log1</code></td>
      <td>ERROR: Can't edit, Log with this name already found</td>
    </tr>
    <tr>
      <td>Valid index, valid string name</td>
      <td><code>2, Chess-NewLog</code></td>
      <td>SUCCESS: Log edited successfully</td>
    </tr>
    <!-- Delete activity log (6 rows) -->
    <tr>
      <td rowspan="6"><strong>Delete activity log</strong></td>
      <td>No activity log exist</td>
      <td rowspan="6">Index</td>
      <td><code>2</code></td>
      <td>ERROR: No activities logs found to delete</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>above activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Invalid Integer format</td>
      <td><code>ab, $, +</code></td>
      <td>ERROR: Format Exception - Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range</td>
      <td><code>-1 or any value below 0</code></td>
      <td>ERROR: Argument out of range Exception</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Running</td>
      <td><code>1</code></td>
      <td>ERROR: Can't delete activity log, Timer is running currently</td>
    </tr>
    <tr>
      <td>Valid index, Timer Status = Stopped</td>
      <td><code>1</code></td>
      <td>SUCCESS: Activity log Deleted successfully</td>
    </tr>
  </tbody>
</table>
