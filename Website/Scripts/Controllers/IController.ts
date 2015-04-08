module Controllers
{
    export interface IControllerScope extends ng.IScope
    {
        to_trusted: Function;
    }

    export interface IControllerTimeout extends ng.ITimeoutService
    {
    }

    export interface IControllerHttp extends ng.IHttpService
    {
    }

    export interface IControllerSce extends ng.ISCEService
    {
    }

    export interface IControllerQ extends ng.IQService
    {
    }

    export interface ILocationService extends ng.ILocationService
    {
    }

    export interface ILocationProvider extends ng.ILocationProvider
    {
    }

    export interface IWindowService extends ng.IWindowService
    {
    }

    export interface ICacheFactoryService extends ng.ICacheFactoryService
    {
    }
}