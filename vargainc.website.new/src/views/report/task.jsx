import React from 'react'
import createReactClass from 'create-react-class'
import BaseView from 'views/base'
import axios from 'axios'

export default createReactClass({
    mixins: [BaseView],
    onClose: function () {
        this.publish('showDialog')
        return false
    },
    render: function () {
        let roles = ['Walker', 'Driver', 'Auditor']
        return (
            <div>
                <button className="close-button" onClick={this.onClose}>
                    <span aria-hidden="true">×</span>
                </button>
                <table className="table-scroll">
                    <caption>
                        Campaign: {this.props.campaignName}
                        <br />
                        Task #: {this.props.taskName}
                    </caption>
                    {roles.flatMap((role) => {
                        let summary = this.props.reports?.[role]?.summary
                        let detail = this.props.reports?.[role]?.detail
                        if (!summary && !detail) {
                            return null
                        }
                        let summaryElements = this.renderSummary(role, summary)
                        let detailElements = this.renderDetail(role, detail)
                        return summaryElements.concat(detailElements)
                    })}
                </table>
            </div>
        )
    },
    renderSummary: function (role, data) {
        return [
            <tbody key={`${role}-head`}>
                <tr>
                    <th colSpan={12}>{role}</th>
                </tr>
                <tr>
                    <th>Campaign</th>
                    <th>Actual</th>
                    <th>High</th>
                    <th>Low</th>
                    <th>YTD(optional)</th>
                    <th>Actual</th>
                    <th>High</th>
                    <th>Low</th>
                    <th>Lifttime(optional)</th>
                    <th>Actual</th>
                    <th>High</th>
                    <th>Low</th>
                </tr>
            </tbody>,
            <tbody key={`${role}-body`}>
                <tr>
                    <td>Avg.Speed(MPH):</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.SpeedAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.SpeedHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.SpeedLow)}</td>
                    <td>Avg.Speed(MPH):</td>
                    <td className="text-right">{this.formatNumber(data?.year?.SpeedAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.SpeedHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.SpeedLow)}</td>
                    <td>Avg.Speed(MPH):</td>
                    <td className="text-right">{this.formatNumber(data?.all?.SpeedAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.SpeedHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.SpeedLow)}</td>
                </tr>
                <tr>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.GroundAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.GroundHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.GroundLow)}</td>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.year?.GroundAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.GroundHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.GroundLow)}</td>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.all?.GroundAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.GroundHigh)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.GroundLow)}</td>
                </tr>
                <tr>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.StopAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.StopHigh, 0)}</td>
                    <td className="text-right">{this.formatNumber(data?.campaign?.StopLow, 0)}</td>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.year?.StopAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.StopHigh, 0)}</td>
                    <td className="text-right">{this.formatNumber(data?.year?.StopLow, 0)}</td>
                    <td>Avg.Ground Covered(MILES):</td>
                    <td className="text-right">{this.formatNumber(data?.all?.StopAvg)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.StopHigh, 0)}</td>
                    <td className="text-right">{this.formatNumber(data?.all?.StopLow, 0)}</td>
                </tr>
            </tbody>,
        ]
    },
    renderDetail: function (role, data) {
        return data.flatMap((item) => {
            return [
                <tbody key={`${role}-${item.userId}-head`}>
                    <tr>
                        <th colSpan={12} className="text-left">
                            {item.fullName}
                        </th>
                    </tr>
                    <tr>
                        <th>Name</th>
                        <th>Actual</th>
                        <th>High</th>
                        <th>Low</th>
                        <th>YTD(optional)</th>
                        <th>Actual</th>
                        <th>High</th>
                        <th>Low</th>
                        <th>Lifttime(optional)</th>
                        <th>Actual</th>
                        <th>High</th>
                        <th>Low</th>
                    </tr>
                </tbody>,
                <tbody key={`${role}-${item.userId}-body`}>
                    <tr>
                        <td>Avg.Speed(MPH):</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.SpeedAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.SpeedHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.SpeedLow)}</td>
                        <td>Avg.Speed(MPH):</td>
                        <td className="text-right">{this.formatNumber(item?.year?.SpeedAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.SpeedHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.SpeedLow)}</td>
                        <td>Avg.Speed(MPH):</td>
                        <td className="text-right">{this.formatNumber(item?.all?.SpeedAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.SpeedHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.SpeedLow)}</td>
                    </tr>
                    <tr>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.GroundAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.GroundHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.GroundLow)}</td>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.year?.GroundAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.GroundHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.GroundLow)}</td>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.all?.GroundAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.GroundHigh)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.GroundLow)}</td>
                    </tr>
                    <tr>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.StopAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.StopHigh, 0)}</td>
                        <td className="text-right">{this.formatNumber(item?.campaign?.StopLow, 0)}</td>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.year?.StopAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.StopHigh, 0)}</td>
                        <td className="text-right">{this.formatNumber(item?.year?.StopLow, 0)}</td>
                        <td>Avg.Ground Covered(MILES):</td>
                        <td className="text-right">{this.formatNumber(item?.all?.StopAvg)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.StopHigh, 0)}</td>
                        <td className="text-right">{this.formatNumber(item?.all?.StopLow, 0)}</td>
                    </tr>
                </tbody>,
            ]
        })
    },
})
