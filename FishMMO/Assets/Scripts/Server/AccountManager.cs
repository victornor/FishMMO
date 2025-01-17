﻿using FishNet.Connection;
using System.Collections.Generic;

namespace FishMMO.Server
{
	/// <summary>
	/// AccountManager maps connections to usernames and usernames to connections. This is a helper class. Adding a connection should only be done in your authenticator.
	/// </summary>
	public static class AccountManager
	{
		public static Dictionary<NetworkConnection, string> ConnectionAccounts = new Dictionary<NetworkConnection, string>();
		public static Dictionary<string, NetworkConnection> AccountConnections = new Dictionary<string, NetworkConnection>();

		public static void AddConnectionAccount(NetworkConnection connection, string accountName)
		{
			ConnectionAccounts.Remove(connection);

			ConnectionAccounts.Add(connection, accountName);

			AccountConnections.Remove(accountName);

			AccountConnections.Add(accountName, connection);
		}

		public static void RemoveConnectionAccount(NetworkConnection connection)
		{
			if (ConnectionAccounts.TryGetValue(connection, out string accountName))
			{
				ConnectionAccounts.Remove(connection);
				AccountConnections.Remove(accountName);
			}
		}

		public static void RemoveAccountConnection(string accountName)
		{
			if (AccountConnections.TryGetValue(accountName, out NetworkConnection connection))
			{
				ConnectionAccounts.Remove(connection);
				AccountConnections.Remove(accountName);
			}
		}

		public static bool GetAccountNameByConnection(NetworkConnection connection, out string accountName)
		{
			return ConnectionAccounts.TryGetValue(connection, out accountName);
		}

		public static bool GetConnectionByAccountName(string accountName, out NetworkConnection connection)
		{
			return AccountConnections.TryGetValue(accountName, out connection);
		}
	}
}