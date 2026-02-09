# AzureResumeFrontEnd

Static HTML/CSS/JS resume website hosted on Azure Static Web Apps (or Azure Blob Storage).

## Features

- Collapsible resume sections (click headings to expand/collapse)
- Visitor counter powered by an Azure Functions backend + Cosmos DB
- Responsive layout for mobile and desktop
- Background image with translucent card design

## Running Locally

Open `index.html` in any browser, or serve with a local HTTP server:

```bash
# Python
python -m http.server 8080

# Node (npx)
npx serve .
```

## Configuration

The API URL for the visitor counter is set in `main.js` via the `functionApi` variable. Update it to point to your deployed Azure Function endpoint for production.

## File Structure

| File             | Purpose                                    |
|------------------|--------------------------------------------|
| `index.html`     | Resume markup and page structure           |
| `styles.css`     | Styling, responsive design, print styles   |
| `main.js`        | Section toggle logic and visitor counter   |
| `Background3.jpg`| Background image referenced by CSS         |
