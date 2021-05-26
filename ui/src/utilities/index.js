export function handleChange(setState, event) {
  const { name, value } = event.target;
  setState((prevState) => ({
    ...prevState,
    [name]: value,
  }));
}

export function getFormattedDate(date) {
  const dateString = date.toISOString().replace("T", " ");
  return dateString.substr(0, dateString.lastIndexOf("."));
}

export function getLocaleDateTime(date) {
  return date.toLocaleString();
}

export function getTitleCase(key) {
  return key
    .replace(/([A-Z])/g, " $1")
    .replace(/^./, (str) => str.toUpperCase());
}

export function getPascalCase(string) {
  return `${string}`
    .replace(new RegExp(/[-_]+/, "g"), " ")
    .replace(new RegExp(/[^\w\s]/, "g"), "")
    .replace(
      new RegExp(/\s+(.)(\w+)/, "g"),
      ($1, $2, $3) => `${$2.toUpperCase() + $3.toLowerCase()}`
    )
    .replace(new RegExp(/\s/, "g"), "")
    .replace(new RegExp(/\w/), (s) => s.toUpperCase());
}

export function getCamelCase(key) {
  return key
    .replace(/(?:^\w|[A-Z]|\b\w)/g, function (word, index) {
      return index === 0 ? word.toLowerCase() : word.toUpperCase();
    })
    .replace(/\s+/g, "");
}
