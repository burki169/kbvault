<?php
if( isset($_FILES) && $_FILES['file']['name'] != "" )
{

print_r($_FILES);
//echo file_get_contents($_FILES['file']['tmp_name']);


   copy( $_FILES['file']['name'], "/var/www/html" ) or
           die( "Could not copy file!");
}

?>
<html>
<head>
<title>Uploading Complete</title>
</head>
<body>
<h2>Uploaded File Info:</h2>
<ul>
<li>Sent file: <?php echo $_FILES['file']['name'];  ?>
<li>File size: <?php echo $_FILES['file']['size'];  ?> bytes
<li>File type: <?php echo $_FILES['file']['type'];  ?>
</ul>

<h3>File Upload:</h3>
Select a file to upload: <br />
<form action="upload.php" method="post"
                        enctype="multipart/form-data">
<input type="file" name="file" size="50" />
<br />
<input type="submit" value="Upload File" />
</form>

</body>
</html>
