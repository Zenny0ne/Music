import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable, ErrorHandler } from "@angular/core";
import { catchError, Observable, tap, throwError } from "rxjs";
import { ApiResponse } from "../models/api-response";

@Injectable({
    providedIn: 'root'
})

export class UserService{

    private baseUrl = "http://localhost:5291/api/user";
    private httpClient = inject(HttpClient);
    private token = "token";

    registerUser(data: FormData) : Observable<ApiResponse<string>>{
        return this.httpClient
        .post<ApiResponse<string>>(`${this.baseUrl}/register`, data)
        .pipe(catchError(this.handleError), 
            tap(response => {
                localStorage.setItem(this.token, response.data);
            })
        );
    }

    loginUser(data:FormData) : Observable<ApiResponse<string>>{
        return this.httpClient.post<ApiResponse<string>>(`${this.baseUrl}/login`, data).pipe(tap(response => {localStorage.setItem(this.token, response.data);}));
    }

    profile() : Observable<ApiResponse<string>>{
        return this.httpClient.get<ApiResponse<string>>(`${this.baseUrl}/profile`);
    }

    profileUpdate(data:FormData) : Observable<ApiResponse<string>>{
        return this.httpClient.put<ApiResponse<string>>(`${this.baseUrl}/profile/update`, data);
    }

    profileDelete() : Observable<ApiResponse<string>>{
        return this.httpClient.delete<ApiResponse<string>>(`${this.baseUrl}/profile/delete`);
    }

    addFavouriteSong(songId: string) : Observable<ApiResponse<string>>{
        return this.httpClient.put<ApiResponse<string>>(`${this.baseUrl}/favouritesong/${songId}`,null);
    }

    addFavouriteAlbum(albumId: string) : Observable<ApiResponse<string>>{
        return this.httpClient.put<ApiResponse<string>>(`${this.baseUrl}/favouritealbum/${albumId}`,null);
    }

    addFavouriteArtist(artistId: string) : Observable<ApiResponse<string>>{
        return this.httpClient.put<ApiResponse<string>>(`${this.baseUrl}/favouriteartist/${artistId}`,null);
    }

    private handleError(error: HttpErrorResponse): Observable<never> {
        let errorMessage = 'An unknown error occurred.';
        if (error.error) {
            if (typeof error.error === 'object' && error.error.message) {
                errorMessage = error.error.message;
            } else if (typeof error.error === 'string') {
                errorMessage = error.error;
            }
        }
        console.error('Error from backend:', errorMessage);
        return throwError(() => new Error(errorMessage));
    }

    get getAccessToken():string|null{
        return localStorage.getItem(this.token);
    }

    logout(){
        localStorage.removeItem(this.token);
        localStorage.removeItem('user');
    }

}