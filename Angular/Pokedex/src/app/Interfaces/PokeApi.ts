export interface Data{
  count: number,
  next: string,
  previous: number,
  result: Resultado[]
}

export interface Resultado{
  name: string,
  url: string
}
