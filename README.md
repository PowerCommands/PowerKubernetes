# Power Kubernetes Client
Kubernetes PowerCommands client that runs "on top" of your kubectl CLI tool, with the purpose to simplify standard things that could be hard to remember. The first thing you need to do is make sure that the startup project is the **PainKiller.PowerCommands.PowerCommandsConsole** project.

## Write standard kubectl commands
You write kubectl commands with the alias k just like any console application like the cmd prompt or powershell. This will change the namespace of the current context to default.
```
k config set-context --current --namespace=default
```
## Write standard docker commands
I am running my kubernetes kluster with Docker Desktop, sometimes it is useful to just run a docker container, therefore I have also added that opportunity just run docker commands as you would in a cmd prompt. 

If you just like me run Docker Desktop you can start that service with this commando (you need to have the path setup properly in the config file). (I don't start Docker Desktop when the machine start to safe some CPU and memory consumption.)
```
dockerdesktop
```
## Bonus commandos

## Publish
With this commando you can publish or delete your kubernetes "projects". A project in this context means a bunch of kubernetes manifest files in yaml format and if you want a bunch of PowerCommand specific files to run kubectl commands, for example to start port forwarding proxy or open a browser with a specific url. They are group together by you or someone else because they should be deployed together for som reason.

If you have clone this repo and started your [Power Kubernetes Client](./src/) you can start with find out witch projects you have by first navigate with command ```cd``` to your manifests directory and then run this command.
```
publish
```
This will you give you something like this as an result.

![Alt text](/docs/images/publish_navigate.png?raw=true "Navigate to manifests")

Note that I have stored a bookmark to the manifests directory in the **PowerCommandsConfiguration.yaml** file and using that with the ```cd``` command to navigate faster. Now that the Power Kubernetes client has the right working directory, now you can run this command to publish something, lets publish the Dashboard.
```
publish dashboard
```
The following is happening, the kubernetes manifest files (*.yaml) in this [manifests/dashboard](./manifests/dashboard/) directory will be processed by the kubectl apply in alphabetical order.  For the dashboard project it is three files that creates a deployment, an admin user and a role binding in the kubernetes kluster.

Then the json files will be processed, also in alphabetical order, this will 

- Start a proxy (and waiting in 5 seconds before starting it)
- Fetch the token for the earlier created admin-user
- Open the admin in a browser
- Change the namespace for the current kubernetes context

**Delete a publication**
You can use the same command but with the option flag --delete and you need to provide the namespace name with option flag --namespace like this.
```
publish dashboard --delete --namespace kubernetes-dashboard
```
## Namespace
You could change namespace for the current context like this:
```
namespace argocd
```
## Base64
To encode a stored kubernetes secret you can use this command, the example is for ArgoCD, the context namespace must be argocd, used the namespace commando from the example above.
```
base64 --secret argocd-initial-admin-secret --data password
```
## Start Docker Desktop
With the start command you make sure that the Docker Desktop service is running so that your kubernetes cluster is available.
```
start
```
Make sure that the path to your Docker Desktop installation is correct in the **PowerCommandsConfiguration.yaml** file.

# Configuration
## PowerCommandsConfiguration.yaml
The configuration file is located in the [PainKiller.PowerCommands.KubernetesCommands](./src/PainKiller.PowerCommands.KubernetesCommands/) project and it set to overwrite the existing one every time the projects is built. There are some configurations in this file that is good to know a little bit more about.
```
pathToDockerDesktop: C:\Program Files\Docker\Docker
bookmark:
    bookmarks:
    - name: manifests
      path: C:\Repos\Github\PowerKubernetes\manifests
      index: 0
```
### pathToDockerDesktop
Not that important, but if you do not start Docker Desktop when you starting your computer and want to start it with the Power Kubernetes Client ```start``` command this configuration must be the path to your Docker Desktop installation directory.
### bookmark
Here you can place "bookmarks" to your favorite places on your disk, you can navigate to them with the ```cd``` command using the --bookmark option and use name or the index, like this (both examples will work just the same).
```
cd --bookmark 0
cd --bookmark manifests
```
## Project manifest yaml files
Lets have a look at the files in the [manifests/minio](./manifests/minio/) directory.
```
minio-01-namespace.yaml
minio\minio-02-pvc.yaml
minio-03-deployment.yaml
minio-04-port-forward.json
minio-05-open-admin.json
minio-06-set-namespace-context.json
```
The yaml file is kubernetes manifest files that will be run ```kubectl apply -f <filename>``` in alphabetical order when you run the publish command as described earlier in this document. They must of course be valid file that could be applied with kubectl. Notice that the Power Kubernetes console does not have any own functionality for this, the Power Kubernetes just executes this commands as if you have typed them in a cmd prompt and hit enter.

You must have set up your environment so that you have a valid kubectl client, I have tried this with a Docker Desktop kubectl client and with the client that VMware developed for their Tanzu kubernetes platform.

### Process metadata files (or what about the *.json files?)
When you are setting up a kubernetes project locally on your machine to try new exiting kubernetes deployments you always end up with some actions that you need to take after the kubernetes manifest files is deployed to the cluster. You may need to port forwarding, decode some default secret, start your browser with a specific url. 

All this stuff you define in a Power Kubernetes specific file that I call **process metadata files**, they are not only intended to use with this Power Command Client, it could be used with any [Power Commands](https://github.com/PowerCommands/PowerCommands2022) project. 

Lets continue examine the minio project, first json file is the **minio-04-port-forward.json** file, and it looks like this.
I have commented each end every property, this comments does not belong to the original file.
```
{
  //Name is the application that should execute, in this case it should be kubectl or something describing when url is set to something
  "Name": "kubectl",

  //Args is the arguments to the kubectl command, leave empty if url is set to something
  "Args": "port-forward pod/minio 9000 9090 -n minio-dev",

  //This is for information purposes only, will be printed out to the console when this file is processed by the publish command.
  "Description": "kubectl port-forward pod/minio 9000 9090 -n minio-dev",

  //UseShellExecute means that a new cmd prompt window should be opened, you need that when you port forward or start a proxy of any kind.
  "UseShellExecute": true,

  //DisableOutputLogging should be set to true when our are printing out access tokens or decode kubernetes secret.
  "DisableOutputLogging": false,

  //UseReadline must be set to true if you want to print out the result of the kubectl command that you run, like a access token or a kubernetes secret
  "UseReadline": false,

  //Base64Decode must be set to true if you want to decode the output from the kubectl command, use when you want to show a password in clear text, use with caution of course.
  "Base64Decode": false,

  //WaitSec means that the client will wait the given number of seconds before execute the command, useful when you want to give the machine some time to spin upp everything before you continue.
  "WaitSec": 30,
  
  //If url is set to a value, the client will open a browser with the given Url.
  "Url": ""
}
```
I think this all you need to get you started, there are som example projects that you could look at and copy the structure from.

# Good luck!