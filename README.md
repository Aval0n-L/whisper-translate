# whisper-translate
Real-time speech recognition and translation with OpenAI Whisper

# Project structure:
whisper-translate/  
├── Dockerfile  
├── whisper-translate.client  
│   └── ( Angular project )  
├── src/  
│   └── WhisperTranslate/  
│       └── WhisperTranslate.csproj  
└── test/  
    └── WhisperTranslate.Tests/  
        └── WhisperTranslate.Tests.csproj  


# Docker
## Build
-t whisper-translate:latest — tag (name) for Docker image.  
. — Docker specifies that the context build is in the current directory (where the Dockerfile is located).  

docker build -t whisper-translate:latest .  

## Run
-d — runs a container in the background.  
-p 5000:80 — forwards port 80 inside the container to port 5000 on your machine. You can configure the ports as you wish.  
--name whisper-translate-container — specifies a name for the container.  
  whisper-translate-service:latest — the name and tag of the Docker image you created in the previous step.  

docker run -d -p 5000:8080 --name whisper-translate-container whisper-translate:latest  

## Check 
docker ps  
curl http://localhost:5000/api/ping  

## Logs
docker logs whisper-translate-service  

## Stop and Delete
docker stop whisper-translate-container  
docker rm whisper-translate-container  

## Remove Image
docker rmi whisper-translate-service:latest  


# Install curl inside a container

1. Go inside the container and run  
        docker exec -it whisper-translate-container /bin/bash  
2. Install curl   
        apt-get update  
        apt-get install -y curl  
3. Check  
        curl http://localhost:8080/api/ping  
