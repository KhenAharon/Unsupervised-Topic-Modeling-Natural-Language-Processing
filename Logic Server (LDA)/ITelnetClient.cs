namespace Client.Model
{
    /*
    ITelenetClient
        Interface for connection to server
    */
    public interface ITelnetClient
    {
        /*
        connect
            get ip,port
            create connection to server according ip and port
        */
        void connect(string ip, int port);
        /*
        disconnect
            close connection to server.
        */
        void disconnect();
        /*
        write
            get data and write to server
        */
        void write(string command);
        /*
        read
            read data from server
        */
        string read();
    }
}