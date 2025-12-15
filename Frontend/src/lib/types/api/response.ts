import { ProblemDetails, ValidationProblemDetails } from "./error";

export type ErrorResponse = {
    ok: false;
    status: number;
    data: ProblemDetails;
}

export type BadRequestResponse = {
    ok: false;
    status: 400;
    data: ValidationProblemDetails;
}

export type OkResponse<T> = {
    ok: true;
    status: number;
    data: T|null;
}

export type ApiResponse<T> = OkResponse<T> | BadRequestResponse | ErrorResponse;