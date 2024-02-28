
#  About this Tool
MS Learn : https://learn.microsoft.com/en-us/azure/architecture/example-scenario/devops/automated-api-deployments-apiops
APIOps applies the concepts of DevOps to Azure API Management. This enables everyone involved in the lifecycle of API design, development, and deployment with self-service and automated tools to ensure the quality of the specifications and APIs that they're building. APIOps places the Azure API Management infrastructure under version control to achieve these goals. Rather than making changes directly in API Management portal, most operations happen through code changes that can be reviewed and audited. In this section, we include links to both a complementary Guide and Wiki to get you started with the tool.

Please bear in mind that APIOPS is designed to facilitate the promotion of changes across different Azure API Management (APIM) instances. While the animation below illustrates changes within the same instance, it's important to note that you can effortlessly apply your modifications across various Azure APIM instances using the supported configuration system. We advise taking some time to explore the [wiki](https://github.com/Azure/apiops/wiki/Configuration) and [documentation](https://azure.github.io/apiops/apiops/5-publishApimArtifacts/apiops-azdo-4-1-pipeline.html) to grasp the functioning of configuration overrides when promoting changes across different environments.

![](assets/gifs/ApiOps.gif)

# Main idea:
To show how to set up the pipelines for API management using API Ops, a concept that combines API management and DevOps.
# •	The steps involved in the API Ops solution: 
The API Ops solution involves three main steps: extracting the API definitions, policies, diagnostics, and other information from the dev instance of API management; creating a pull request to review and approve the changes; and publishing the changes to the higher environments of API management using Azure DevOps pipelines.
# •	The tools and resources needed for the API Ops solution: 
The API Ops solution requires minimum two API management instances for dev and prod environments; a git repo that contains the tools and YAML files for the extractor and publisher pipelines; a service connection, a variable group, and two environments in Azure DevOps; and a configuration file for each environment that specifies the API management name and other settings.
# •	The benefits of the API Ops solution: 
The API Ops solution offers several benefits, such as placing the API management infrastructure under version control, enabling review and audit of the changes, and ensuring consistency and quality across the environments.

# Outline:
The extractor pipeline is going to pull API definitions policy information and diagnostic settings etc from Dev instance of our API management and create a PR for us once which after been reviewed and approved it's going to be merged to the main branch which is going to kick off the publisher pipeline which is going to deploy to the prod APIM.
API Ops is a similar concept like git Ops or DevOps where you are combining two practices together or two methodologies together and Apios is going to place the API management infrastructure definitions under Version Control so when you make a changes to your API management instance these changes are going to be extracted as code to be reviewed and audited and once have been approved it's going to be deployed to the higher environments like UAT and prod.

# Steps:
1.	Create a prod API management instance preferably in a new prod resource Group.
2.	Create a new DevOps project.

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/0d00fdf3-bbf7-4fbc-906d-0e057a763d49)
3.	From this GitHub repo -https://github.com/Azure/apiops for the API Ops solution 
go to tags and go to latest version and download azure_devops.zip
4.	Extract this folder and you can see there is a tools/pipelines folder and inside yaml files which are going to be used for extractor Pipeline and publisher pipeline.
5.	Go to repository and initialize apiops repo and create a new folder for tools then Pipelines and add readme.md file and commit this change.
![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/ad25d6bd-b06b-4c3f-87b6-e76e1d910ef6)

6.	In pipelines folder upload the three yaml files
![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/25cacb2f-5480-4881-a289-a7960c557b77)

7.	Create one more yaml file here configuration.prod.yaml like https://github.com/Azure/apiops/blob/main/configuration.prod.yaml
8.	This configuration file specifies certain configuration that are specific for different environments.
9.	Edit this file first and clean everything up just leaving the main tags, however you need to change the API name to be the name of your prod instance so copy the name of prod API management and paste it in configuration.prod.yaml 
![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/6ec4abae-cf78-46db-92be-d952543e11f6)

10.	Create a service connection so go to the project settings and create one with connection type to be Azure resource manager, use service principle (automatic) and choose the subscription where you have your API management instance created and select the Dev Resource Group, call the service connection apimconnection and check on the checkbox to Grant access to all Pipelines.
11.	Go to pipelines environments and create a prod environment and add an approver in this environment.

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/3fbc0e9d-31e7-408e-9d60-6daac92415d1)

12.	Go to library and create a variable group but this variable group must be a specific name and to know what it is head back to the repo and see how we should be calling this variable group if you got o extractor.yaml you should be able to see under variables section here the group name that we should be using in the library. if you want to change it you need to change it here first. Keep the original as specified.
Select Pipelines > Library > + Variable group. Enter apim-automation as the name of the variable group.

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/998386f0-2736-478b-8cb6-0eda69b0d839)

13.	Add the variables in the group according to the documentation https://github.com/Azure/apiops/blob/main/docs/apiops/3-apimTools/apimtools-azdo-2-3-new.md
14.	Create the extractor pipeline, using the existing yaml file in the Azure repository, Rename the pipeline and call it Extractor and run the pipeline.
Select New Pipeline -> Azure Repos Git.
Select the APIOpsDemo repository and choose Existing Azure Pipelines Yaml file. Enter the path of the yaml file as /tools/pipelines/run-extractor.yaml
15.	Before we run the Extractor pipeline, we must grant Contributor permission to the Build Service account in ApiOpsDemo repository. The Contributor permission will be required for the pipeline to create a pull request. Navigate to Project Settings > Repositories. Select ApiOpsDemo repository and assign the permissions as shown in the below figure:

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/ab71bbad-f14d-476a-bc93-a1a6b0032424)

16.	Navigate to the Extractor pipeline and select Run Pipeline. Add the parameter values as shown in the below image and select Run:
![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/69b0479c-6ee2-4830-8e98-96daf2082fcd)

17.	When the pipeline runs successfully you will notice output of the first stage which is what's been exported from Dev API management so when we run our extractor pipeline the extractor pulls out all of this information from Dev API management then the extractor goes ahead and creates a new pull request with this change requesting to merge all of these artifacts into the main branch so these are simply the files that we have just exported and we are ready to be added to our repo but before approving and merging these changes to the main branch you need to set up the publisher pipeline first.

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/64799c06-da16-43d4-a0ad-15d170cedd68)

18.	Add a new pipeline and from the yaml file comment out the dev stage, rename our apim Pipeline and let's call it publisher.
Now create the publisher pipeline using the same steps as above. This time you will be selecting /tools/pipelines/run-publisher.yaml file. Rename the pipeline as Publisher
19.	Approve the pull request and complete it by merging it to the main branch and as soon it's going to be merged the publisher pipeline is going to be automatically kicked off to deploy our changes to prod.
20.	You might get and error as you can see here, we need to install this tool from Visual Studio Marketplace, open Visual Studio Marketplace so we can install replace token as it's been highlighted let's get back to the Pipeline and let's run it one more time.

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/843cbd41-29ea-4fff-8a98-ce6255a4bb5b)

![image](https://github.com/damayantiazure/apiops-devops/assets/92169356/4370cf4b-49ae-418b-8f5b-d2f49e82b979)

21.	The pipeline waits for the approval before deploying to the Prod environment. Go ahead and approve the deployment. You will now see that pipeline resumes and deploys the changes to Prod environment. The API resources in the Prod environment should now be in sync with the Dev environment.

Links:
Automated API deployments using APIOps - Azure Architecture Center | Microsoft Learn
https://github.com/Azure/APIOps





