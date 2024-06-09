import { Injectable } from '@angular/core';
import { Resultado } from '../Interfaces/PokeApi';

@Injectable({
  providedIn: 'root',
})
export class PokemonService {
  constructor() {}

  async GetByPage(): Promise<Resultado[]> {
    let res = await fetch(
      'https://pokeapi.co/api/v2/pokemon/?limit=20&offset=20'
    );

    let resJson = await res.json();

    if (resJson.results.lenght > 0) {
      return resJson;
    } else {
      return [];
    }
  }

  GetById() {
    //https:///pokeapi.co/api/v2/pokemon
  }

  GetDescription() {}
}
