declare module '*.module.css' {
  const classes: Readonly<Record<string, string>>;
  export default classes;
}

declare module '*.svg?raw' {
  const content: string;
  export default content;
}

// Side-effect stylesheet imports (tokens.css, base.css).
declare module '*.css';
