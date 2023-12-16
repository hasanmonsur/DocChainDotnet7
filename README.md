
# DocsChain Asp.Net Core 7

> Disclaimer: This software is only at proof-of-concept stage, don't
> pretend this software to be ready for production usage. It works on
> the designed path, but it needs a lot more development. You have been
> warned !


## Project Goal 
DocsChain Asp.Net Core 7 is a private blockchain to store and certify documents, designed to be very easy to use and deploy.
You can create small peer-to-peer networks of documents contributors. It doesn't have a mining algorithm and so it cannot be used in a public network. However it implements the basic features of a working blockchain.
The node & chain is stored as a serialised .dc file in the **Config** & **DataFiles** Directory along with another file, with network peers. The chain doesn't include uploaded documents. Documents are stored into the **FileStorage** Directory using the block Guid.
It can work with required extensions (doc,docx,txt,xls,xlsx,png,jpg,jpeg,gif,csv)

## Tech use DocsChain Asp.Net Core 7
Code is based on **Asp .Net 7 + c# + WebApp & WebApi** 
**Controllers\WebAppController.cs** is for the GUI Action methods in **DocChainWeb** used for only clint as access the private block chanin.
**Controllers\ChainController.cs** is used by WebApis controller in **DocsChain** is used for managed private blockchain network and brodcust all other block.

## I have take help of git project
https://github.com/frontegi/DocsChain.git which is very helpfull to customize and upgradation this project.
## Dependencies of this project
Only the GUI has dependency with Bootstrap, Jquery, and for PDF viewing, the great plugin [iziModal by Marcelo Dolce](http://izimodal.marcelodolce.com/) .
## Inspiration
[I read an article](https://medium.com/@lhartikk/a-blockchain-in-200-lines-of-code-963cc1cc0e54) on Medium.com by **Lauri Hartikka**, explaining how to build a basic blockchain in Javascript without all the hassles of blockchain players.
I thought it was the time to create my own implementation. Thanks Lauri for the great article !
Here are some Highlights from his article:

> The basic concept of 
> [blockchain](https://en.wikipedia.org/wiki/Blockchain_%28database%29) 
> is quite simple: a distributed database that maintains a continuously
> growing list of ordered records.


## How DocsChain works
This project contain two project by a solution When you first run a **DocsChain.sln** with multiple project, it will check locally for **Config** & **DataFiles** folder and **FileStorage** folder.
If missing, they will be created, along with an empty chain.
GUI has no authentication, you can interact immediately with the node (ARGH :-) ).
The GUI is absolutely basic, built with Bootstrap 4.

### GUI has three pages:

 - **Documents Upload**
	Here you can preview (only PDF) and download documents in the blockchain.
	You'll see immediately only the **Genesis Block**, built during node bootstrap.
	Moreover, you can upload a pdf documents and add it to the chain.
	If you have built and connected more nodes, the documents will be broadcasted to the other peers.
	Imagine to have 3 offices in your Intranet, and you want the to "certify" a document (data and upload timing).
	Every node can contribute to the Chain, adding any new documents when required. DocsChain limit is now that, without login, I cannot write to the chain who uploaded a document.
	Here you can find when the DocsChain has been created the first time (Genesis Date) 
    You can see the list of Peers connected.

 - **System Status**
    This function shows all node status, as a result you can easily know which node is active and which IP and port is active for brodcust by sync.

 - **Chain Management**
    In this page use for config for node and function **Bootstrap Node** and for new document upload go **Add New Documents** also there have reset the full network **Reset DocsChain Nodes** .


## How to run the POC

 1. ## The easy way, Visual Studio 2022

Download the project and just Run it with **Visual Studio 2022**.
If everything works fine, you'll have a consolle opening with logs and the browser will open on the Chain Node Url . If browser is not opening , go to the url **http://[YourNodeIP]:[YourNodePort]**
If you want customize **Node Name, Listening IP or Port**, edit **appsettings.json** file:

     "NodeIdentification": {
        "Name": "Node 10",
        "ListeningPort": "5979",
        "IPEndpoint": "http://localhost:5979"
    	}

Keep **ListeningPort** and the **IPEndpoint** port the same (this is a POC, it'll be optimized later...)
It is mandatory to use the real IP of Network Interface (**ipconfig /all** on command line) if you want to make the POC work in your intranet on different machines.
You cannot  use **localhost** or **127.0.0.1** here. You can do it (not tested) only if you're going to run **multiple DocsChain nodes** on the same machine . 

 2. ## Multiple Nodes Way, with command line
If you are a brave and you want to showcase **DocsChain** with **multiple nodes** (on the same machine, never tried on different machines), I suggest the following approach (*remember to replace the IP below with your own IP and desired port*):
 - Right Click on Project Root Node and Publish to a Folder.  (es:c:\DocsChainBase\)
 - Clone the Folder as c:\DocsChain5010
 - Clone the Folder as c:\DocsChain5020
 - Open c:\DocsChain5010\appsettings.json and set

     "NodeIdentification": {
	            "Name": "Node 5010",
                "ListeningPort": "5010",
                "IPEndpoint": "http://localhost:5010"
            	}

 - Open c:\DocsChain5020\appsettings.json and set

     "NodeIdentification": {
        "Name": "Node 5020",
        "ListeningPort": "5020",
        "IPEndpoint": "http://localhost:5020"
    	}

## First Chain
 
 	- Open a Command Line 1 and cd \DocsChain5010\

 	- launch the first node with: `dotnet DocsChain.dll` 

 	- Open the browser and go to http://localhost:5010/ (Rememeber this is my example IP, use yours !!)

## Second Chain
 	- Go to \DocsChain5020\

 	- run `dotnet DocsChain.dll`.The second node will boot

 	- Browse to http://localhost:5020/

 - we have requred other run web application **DocChainWeb** using diffrent port.  for each project then we can use run properly . 
	- Browse to http://localhost:5050/
	- Browse to http://localhost:5060/


### Troubleshooting
How to restart from the beginning when you are going crazy:

 - stop a node
 - delete **Config** & **DataFiles** Folder and **FileStorage** Folder
 - restart the node

### Missing Features

 - User Authentication and user management & Full Client Site Development.
 - Packaging the app as a Windows Service
 - File watcher to keep an eye on documents stored on local filesystem.
 - Auto-Recover documents from other nodes if found a file corrupted (different Hash)
 - General recovery of BlockChain if something is going wrong
 - Supporting more native files viewer in the GUI
 - Search along documents feature
 - Chain File Security

### "Hidden" Command line switches... 
- `dotnet DocsChain.dll --boot http://localhost:5010` (To boot a node without using the GUI)
- `dotnet DocsChain.dll --create-test-chain true` (boot and add immediately sample documents to the chain)
- `dotnet DocsChain.dll --boot http://localhost:5020` (To boot a node without using the GUI)
- `dotnet DocsChain.dll --create-test-chain true` (boot and add immediately sample documents to the chain)
- `dotnet DocChainWeb.dll --boot http://localhost:5050` (To boot a node without using the GUI)
- `dotnet DocChainWeb.dll --create-test-chain true` (boot and add immediately sample documents to the chain)
- `dotnet DocChainWeb.dll --boot http://localhost:5060` (To boot a node without using the GUI)
- `dotnet DocChainWeb.dll --create-test-chain true` (boot and add immediately sample documents to the chain)

## Contributing Guidelines
	anyone can be contribute in this project and also https://github.com/frontegi/DocsChain.git  
	**frontegi/DocChain**
