declare module '*.module.css' {
  const classes: Readonly<Record<string, string>>;
  export default classes;
}

declare module '*.svg?raw' {
  const content: string;
  export default content;
}
