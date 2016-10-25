<?php
//setting header to json
header('Content-Type: application/json');

//database
define('DB_HOST', 'localhost');
define('DB_USERNAME', 'root');
define('DB_PASSWORD', 'password');
define('DB_NAME', 'contact_form');

$mysqli = new mysqli(DB_HOST, DB_USERNAME, DB_PASSWORD, DB_NAME);

if (!$mysqli) {
    die('Connect Error: ' . mysqli_connect_error() . "\n");
}

//query to get data from the table
$query = sprintf("SELECT * FROM `entries` WHERE 1");

//execute query
$result = $mysqli->query($query);

//loop through the returned data
$data = array();
foreach ($result as $row) {
	$data[] = $row;
}

//free memory associated with result
$result->close();

//close connection
$mysqli->close();

//now print the data
print json_encode($data);
?>