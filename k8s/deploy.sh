cd /home/$USER/dotpics/k8s
source .env

VARS='${DOMAIN} ${APP_DOMAIN} ${IMAGE_TAG} ${EMAIL}'

for f in *.yaml; do
  envsubst "$VARS" < "$f" | microk8s kubectl apply -f -
done
