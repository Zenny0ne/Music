import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable, tap } from "rxjs";
import { ApiResponse } from "../models/api-response";

@Injectable({
    providedIn: 'root'
})

export class UserService{

    private baseUrl = "http://localhost:5291/api/user";
    private token = "token";

    isLoggedIn():boolean{
        const token = this.getAccessToken;
        return token !== null && token !== '';
    }

    get getAccessToken():string|null{
        return localStorage.getItem(this.token);
    }

}