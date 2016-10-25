<?php

$link = mysqli_connect("localhost","root","password");
if (!$link) {
    die('Connect Error: ' . mysqli_connect_error() . "\n");
    echo mysql_errno($link) . ": " . mysql_error($link). "\n";
}

mysqli_select_db($link, "contact_form");

if(isset($_REQUEST['submit']))
{
    $errorMessage = "";
    $name=$_POST['name'];
    $profession=$_POST['profession'];
    $technical=$_POST['technical'];
    $birth=$_POST['birth'];

    if (isset($_POST['languages'])) {
        $languages=implode(',', $_POST['languages']);
    }

    $postcode=$_POST['postcode'];
    $email=$_POST['email'];

    // Validation will be added here

    if ($errorMessage != "" ) {
    echo "<p class='message'>" .$errorMessage. "</p>" ;
    } else {
        //Inserting record in table using INSERT query

        $insqDbtb = "INSERT INTO `entries`
        (`fullname`, `profession`, `birthYear`, `skill`, `languages`, `postcode`,`email`)
        VALUES
        ('$name', '$profession', '$birth', '$technical', '$languages', '$postcode', '$email')";
        mysqli_query($link, $insqDbtb) or die("Error: " . mysqli_error($link));
    }
    header( "Location: http://teammlg.uqcloud.net/" );
}
?>
