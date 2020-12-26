## Sbanken to YNAB
This is a tool for exporting transactions from [Sbanken](https://sbanken.no/) to [YNAB](https://www.youneedabudget.com/) (You Need A Budget).

### Getting started
First of all you need to update the `Credentials/SbankenCredentials.cs` and `Credentials/YNABCredentials.cs` files with your credentials. Afterwards you are ready to run the program.

This program requires two arguments; the name of your account in Sbanken which you wish to get transactions from, and the name of your budget in YNAB that you wish to update.

Example: `dotnet run "My account" "My budget"`

You can also pass an optional third argument if you have multiple accounts in your YNAB budget, and you need to specify which one to update. 

Example: `dotnet run "My account" "My budget" "My YNAB account"`

If no third argument is passed, the first account found on your YNAB budget will be used.

### Development
I made this using [VSCode](https://code.visualstudio.com/) and the [Remote Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) extension. For this to work you also need to have [Docker](https://www.docker.com/) running. 

To get started you simply need to clone this repo, open it in VSCode and choose `Reopen in container`.
