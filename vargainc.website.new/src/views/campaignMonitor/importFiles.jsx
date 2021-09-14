import Backbone from 'backbone'
import React from 'react'
import ReactDOMServer from 'react-dom/server'
import 'react.backbone'
import { map } from 'lodash'

export default React.createClass({
    shouldComponentUpdate: function () {
        return false
    },
    onImport: function (taskId, e) {
        e.bubbles = false

        var uploadFile = e.currentTarget.files[0]
        if (uploadFile.size == 0) {
            alert('please select an not empty file!')
            return
        }

        var model = new TaskModel({
                Id: taskId,
            }),
            self = this

        model.importGtu(uploadFile).then(() => {
            alert('import success')
        })
    },
    render: function () {
        var self = this,
            tasks = this.props.tasks || []
        return (
            <div>
                {map(tasks, (taskId) => {
                    return (
                        <input
                            key={`file-import-${taskId}`}
                            type="file"
                            id={`upload-file-${taskId}`}
                            multiple
                            style={{ display: 'none' }}
                            onChange={self.onImport.bind(this, taskId)}
                        />
                    )
                })}
            </div>
        )
    },
})
