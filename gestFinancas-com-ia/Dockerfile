# Build stage
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG API_URL
RUN if [ -n "$API_URL" ]; then \
    sed -i "s|https://api.seudominio.com|$API_URL|g" src/environments/environment.prod.ts; \
    fi
RUN npm run build

# Serve stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
