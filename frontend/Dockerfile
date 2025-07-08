# Stage 1: Build the Angular app
FROM node:22-alpine AS build

WORKDIR /app

# Copy package files and configs
COPY package*.json angular.json tsconfig*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY src/ src/
COPY public/ public/

# Build the Angular app for production
RUN npm run build --configuration=production

# Stage 2: Serve with Nginx
FROM nginx:alpine AS production

# Create SSL directory
RUN mkdir -p /etc/nginx/ssl

# Remove default Nginx files
RUN rm -rf /usr/share/nginx/html/*

# Copy built Angular app (Angular 17+ output structure)
COPY --from=build /app/dist/app/browser/ /usr/share/nginx/html/

# Copy Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy SSL certificates
COPY ssl/localhost.crt /etc/nginx/ssl/localhost.crt
COPY ssl/localhost.key /etc/nginx/ssl/localhost.key

# Set proper permissions for SSL files
RUN chmod 644 /etc/nginx/ssl/localhost.crt && \
    chmod 600 /etc/nginx/ssl/localhost.key

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
