import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable, tap } from "rxjs";
import { ApiResponse } from "../models/api-response";

@Injectable({
    providedIn: 'root'
})

export class UserService{

    private baseUrl = "http://localhost:5291/api/user";
    private httpClient = inject(HttpClient);
    private token = "token";

    register(data:FormData) : Observable<ApiResponse<string>>{
        return this.httpClient
            .post<ApiResponse<string>>(`${this.baseUrl}/register`,data)
            .pipe(tap(response => {
                localStorage.setItem(this.token, response.data);
            }
        ));
    }

    login(email:string, password:string): Observable<ApiResponse<string>>{
        return this.httpClient
            .post<ApiResponse<string>>(`${this.baseUrl}/login`, {email, password})
            .pipe(
            tap(response => {
                localStorage.setItem(this.token, response.data);
            })
        );
    }

    profile(): Observable<ApiResponse<string>>{
        return this.httpClient
            .get<ApiResponse<string>>(`${this.baseUrl}/profile`,{
                headers:{
                    Authorization: `Bearer ${this.getAccessToken}`
                }
            })
            .pipe(tap(response => {
                if(response.isSuccess){
                    localStorage.setItem('user', JSON.stringify(response.data));
                }
            }
        ));
    }

    profileUpdate(){

    }

    profileDelete(){

    }

    addFavouriteSong(){

    }

    addFavouriteAlbum(){

    }

    addFavouriteArtist(){
        
    }

    get getAccessToken():string|null{
        return localStorage.getItem(this.token);
    }

}