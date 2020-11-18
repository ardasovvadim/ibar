import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor() { }


  public saveInRepository(key: string, value: string) {
    localStorage.setItem(key, value);
  }

  public removeFromRepository(key: string) {
    localStorage.removeItem(key);
  }

  public getFromRepository(key: string) {
    return localStorage.getItem(key);
  }

}
