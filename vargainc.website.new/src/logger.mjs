import debug from 'debug'

export default class Logger {
    constructor(module) {
        this._info = debug(`${module}:info`)
        this._info.log = console.info.bind(console)

        this._error = debug(`${module}:error`)
        this._error.log = console.error.bind(console)
    }

    info(...args) {
        let msg = args.reduce((line, p) => {
            if (typeof p !== 'string') {
                line += JSON.stringify(p, null, 4)
            } else {
                line += p
            }
            return line
        }, '')

        this._info.call(null, msg)
    }

    error(...args) {
        let msg = args.reduce((line, p) => {
            if (typeof p !== 'string') {
                line += p.toString()
            } else {
                line += p
            }
            return line
        }, '')

        let error = new Error(msg)
        this._error.call(null, error)
    }
}
