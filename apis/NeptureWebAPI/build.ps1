$imageName = "neptune-webapi"
$tag = "beta2"
$registry = "neptuneimages.azurecr.io"


Write-Output "Login to Azure Container Registry"

$accessToken = az acr login --name $registry --expose-token --output tsv --query accessToken
docker login $registry --username 00000000-0000-0000-0000-000000000000 --password $accessToken

Write-Output "Building Images with Tag '${imageName}:${tag}'"
docker build -t ${registry}/${imageName}:${tag} .

Write-Output "Pushing to '$registry'"
docker push ${registry}/${imageName}:${tag}
