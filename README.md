# EventManagement

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.2.12.

## Launching Instructions

First, clone this repository and run this command in your console - `npm install`.

After that you will need to complete a few setup steps before running the project:

1. Run the Swagger API:
   Ensure the backend (Swagger) is running locally or hosted elsewhere, as the frontend depends on it.

2. Set up Environment Files:
   Remove the `.example` extension from the files in the `src/environments/` folder.

   Update the placeholders with valid URLs for both `apiUrls` and `hubUrls` inside the `environment.ts` and `environment.development.ts` files.

3. Set up Redis Caching:
   Create and run a Docker container with the Redis image.
   Make sure the backend is configured to connect to this Redis container.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
