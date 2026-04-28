// Build-time version pulled from package.json. Used in the on-screen label
// and forms part of the service-worker cacheId so a version bump invalidates
// the precache. Bump package.json's `version` field on each deploy.
export const APP_VERSION: string = __APP_VERSION__;
