// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../models/user.model'; 

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7105/api/users'; // Cambia esto según la URL de tu backend

  constructor(private http: HttpClient) {}

  register(user: UserDto): Observable<any> {
    return this.http.post(`${this.apiUrl}`, user);
  }

  login(user: UserDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, user);
  }
}
