import { Injectable } from '@angular/core';

@Injectable()
export class StorageService {

  constructor() { }

  saveInRepository(key: string, value: string) {
    localStorage.setItem(key, value);
  }

  removeFromRepository(key: string) {
    localStorage.removeItem(key);
  }

  getFromRepository(key: string): string | null {
    return localStorage.getItem(key);
  }

}
