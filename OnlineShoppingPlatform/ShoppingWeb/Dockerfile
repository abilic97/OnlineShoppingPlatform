# Frontend (Angular)
FROM node:16.14.0 AS build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --prod

# Use NGINX to serve the built files
FROM nginx:alpine
COPY --from=build /app/dist/* /usr/share/nginx/html
EXPOSE 80
