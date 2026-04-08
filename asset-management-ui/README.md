# Asset Management UI

Angular frontend for managing assets, employees, software licenses, and status values.

## Current Scope

The application currently supports:

- viewing and creating assets
- viewing and creating employees
- viewing and creating software licenses
- viewing and creating status records

The UI calls the backend API at `http://localhost:8080/api`.

## Routes

- `/assets`
- `/employees`
- `/software-licenses`
- `/status`

## Local Setup

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm start
```

Open the app in the browser at `http://localhost:4200/`.

## Build

Create a production build:

```bash
npm run build
```

Build output is generated in the `dist/asset-management-ui` directory.

## Tests

Run unit tests:

```bash
npm test
```

## Future Implementation

- search, sorting, filtering, and pagination across all lists
- validation and clearer error handling for API failures
- authentication and role-based access control
- environment-based API configuration instead of a hardcoded base URL
- unit, integration, and end-to-end test coverage for core user flows

