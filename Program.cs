using src;

//Here is the url of the connection to mongo
var connection = new Connection("");

//Add your database name to test the connection
if (!connection.IsConnected(""))
    throw new Exception("Client failed to connect to server");

Console.WriteLine("Connection started successfully!");

//Here will be the name of the collection where we will work the operations
connection.GetCollection("");

foreach (var data in connection.GetAll()){
    Console.WriteLine(data);
}
