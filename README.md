# jesusmidget-website

This is the website for [jesusmidget.com](https://jesusmidget.com)

## Technologies & Languages

- TypeScript
- Next.js
- React
- CircleCI
- AWS S3

## Development

To run the development server:

```bash
yarn dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

## Production

1. Just push to any branch.
2. CircleCI will launch a Next.js build.
3. The generated files are then uploaded to a public-accessible AWS S3 bucket, configured to be served as a static website.

## Miscellaneous

### Why Next.js?

This website was originally made in PHP with the CodeIgniter framework. Because it barely had any back-end and that it was tedious to maintain and deploy in production, I wanted to develop it with React instead, while keeping the SEO advantages of a site whose HTML pages are generated server-side.
